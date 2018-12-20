using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace SocketServers
{
	public class ServersManager<C> : IDisposable where C : BaseConnection, IDisposable, new()
	{
		private class EndpointInfo
		{
			public readonly ProtocolPort ProtocolPort;

			public readonly UnicastIPAddressInformation AddressInformation;

			public readonly ServerEndPoint ServerEndPoint;

			public EndpointInfo(ProtocolPort protocolPort, UnicastIPAddressInformation addressInformation)
			{
				this.ProtocolPort = protocolPort;
				this.AddressInformation = addressInformation;
				this.ServerEndPoint = new ServerEndPoint(this.ProtocolPort, this.AddressInformation.Address);
			}
		}

		private object sync;

		private bool running;

		private ThreadSafeDictionary<ServerEndPoint, Server<C>> servers;

		private ThreadSafeDictionary<ServerEndPoint, Server<C>> fakeServers;

		private List<ProtocolPort> protocolPorts;

		private List<UnicastIPAddressInformation> networkAddressInfos;

		private ServersManagerConfig config;

		private int nextPort;

		private Logger logger;

		public event EventHandler<ServerChangeEventArgs> ServerRemoved;

		public event EventHandler<ServerChangeEventArgs> ServerAdded;

		public event EventHandler<ServerInfoEventArgs> ServerInfo;

		public event ServerEventHandlerRef<ServersManager<C>, C, ServerAsyncEventArgs, bool> Received;

		public event ServerEventHandlerRef<ServersManager<C>, ServerAsyncEventArgs> Sent;

		public event ServerEventHandlerVal<ServersManager<C>, C> NewConnection;

		public event ServerEventHandlerVal<ServersManager<C>, C> EndConnection;

		public event ServerEventHandlerVal<ServersManager<C>, C, ServerAsyncEventArgs> BeforeSend;

		public Func<NetworkInterface, IPInterfaceProperties, UnicastIPAddressInformation, bool> AddressPredicate
		{
			get;
			set;
		}

		public Func<ServerEndPoint, IPEndPoint> FakeAddressAction
		{
			get;
			set;
		}

		public Logger Logger
		{
			get
			{
				return this.logger;
			}
		}

		private List<UnicastIPAddressInformation> NetworkAddresses
		{
			get
			{
				if (this.networkAddressInfos == null)
				{
					this.networkAddressInfos = new List<UnicastIPAddressInformation>();
					NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
					NetworkInterface[] array = allNetworkInterfaces;
					for (int i = 0; i < array.Length; i++)
					{
						NetworkInterface networkInterface = array[i];
						if (networkInterface.OperationalStatus == OperationalStatus.Up)
						{
							IPInterfaceProperties iPProperties = networkInterface.GetIPProperties();
							foreach (UnicastIPAddressInformation current in iPProperties.UnicastAddresses)
							{
								if (this.AddressPredicate(networkInterface, iPProperties, current))
								{
									this.networkAddressInfos.Add(current);
								}
							}
						}
					}
				}
				return this.networkAddressInfos;
			}
		}

		public ServersManager(ServersManagerConfig config)
		{
			if (!BufferManager.IsInitialized())
			{
				BufferManager.Initialize(256);
			}
			if (!EventArgsManager.IsInitialized())
			{
				EventArgsManager.Initialize();
			}
			this.running = false;
			this.sync = new object();
			this.protocolPorts = new List<ProtocolPort>();
			this.servers = new ThreadSafeDictionary<ServerEndPoint, Server<C>>();
			this.fakeServers = new ThreadSafeDictionary<ServerEndPoint, Server<C>>();
			this.AddressPredicate = new Func<NetworkInterface, IPInterfaceProperties, UnicastIPAddressInformation, bool>(ServersManager<C>.DefaultAddressPredicate);
			this.FakeAddressAction = new Func<ServerEndPoint, IPEndPoint>(ServersManager<C>.DefaultFakeAddressAction);
			this.config = config;
			this.nextPort = config.MinPort;
			this.logger = new Logger();
		}

		private static bool DefaultAddressPredicate(NetworkInterface interface1, IPInterfaceProperties properties, UnicastIPAddressInformation addrInfo)
		{
			return true;
		}

		private static IPEndPoint DefaultFakeAddressAction(ServerEndPoint endpoint)
		{
			return null;
		}

		public SocketError Start(bool ignoreErrros)
		{
			SocketError result;
			lock (this.sync)
			{
				this.running = true;
				NetworkChange.NetworkAddressChanged += new NetworkAddressChangedEventHandler(this.NetworkChange_NetworkAddressChanged);
				result = this.AddServers(this.GetEndpointInfos(this.protocolPorts), ignoreErrros);
			}
			return result;
		}

		public void Dispose()
		{
			NetworkChange.NetworkAddressChanged -= new NetworkAddressChangedEventHandler(this.NetworkChange_NetworkAddressChanged);
			lock (this.sync)
			{
				this.running = false;
				this.servers.Remove((ServerEndPoint endpoint) => true, new Action<Server<C>>(this.OnServerRemoved));
				this.logger.Dispose();
			}
		}

		public SocketError Bind(ProtocolPort pp)
		{
			SocketError result;
			lock (this.sync)
			{
				this.protocolPorts.Add(pp);
				if (this.running)
				{
					result = this.AddServers(this.GetEndpointInfos(pp), false);
				}
				else
				{
					result = SocketError.Success;
				}
			}
			return result;
		}

		public SocketError Bind(ref ProtocolPort pp)
		{
			SocketError result;
			lock (this.sync)
			{
				if (this.nextPort < 0)
				{
					throw new InvalidOperationException("Port range was not assigned");
				}
				for (int i = 0; i < this.config.MaxPort - this.config.MinPort; i++)
				{
					pp.Port = this.nextPort++;
					SocketError socketError = this.AddServers(this.GetEndpointInfos(pp), false);
					if (socketError != SocketError.AddressAlreadyInUse)
					{
						result = socketError;
						return result;
					}
				}
				result = SocketError.TooManyOpenSockets;
			}
			return result;
		}

		public void Unbind(ProtocolPort pp)
		{
			lock (this.sync)
			{
				this.protocolPorts.Remove(pp);
				if (this.running)
				{
					this.servers.Remove((ServerEndPoint endpoint) => endpoint.Port == pp.Port && endpoint.Protocol == pp.Protocol, new Action<Server<C>>(this.OnServerRemoved));
				}
			}
		}

		public void SendAsync(ServerAsyncEventArgs e)
		{
			Server<C> value = this.servers.GetValue(e.LocalEndPoint);
			if (value == null)
			{
				value = this.fakeServers.GetValue(e.LocalEndPoint);
			}
			if (value != null)
			{
				if (this.logger.IsEnabled)
				{
					this.logger.Write(e, false);
				}
				value.SendAsync(e);
				return;
			}
			e.SocketError = SocketError.NetworkDown;
			this.Server_Sent(null, e);
		}

		public X509Certificate2 FindCertificateInStore(string thumbprint)
		{
			X509Store x509Store = null;
			X509Certificate2 result;
			try
			{
				x509Store = new X509Store(StoreLocation.LocalMachine);
				X509Certificate2Collection x509Certificate2Collection = x509Store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
				if (x509Certificate2Collection.Count > 0)
				{
					result = x509Certificate2Collection[0];
				}
				else
				{
					result = null;
				}
			}
			finally
			{
				if (x509Store != null)
				{
					x509Store.Close();
				}
			}
			return result;
		}

		public bool IsLocalAddress(IPAddress address)
		{
			return this.servers.Contain((Server<C> server) => server.LocalEndPoint.Address.Equals(address) || (server.FakeEndPoint != null && server.FakeEndPoint.Address.Equals(address)));
		}

		private void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
		{
			this.OnServerInfo(new ServerInfoEventArgs(ServerEndPoint.NoneEndPoint, "NetworkChange.NetworkAddressChanged"));
			lock (this.sync)
			{
				this.networkAddressInfos = null;
				IEnumerable<ServersManager<C>.EndpointInfo> infos = this.GetEndpointInfos(this.protocolPorts);
				this.AddServers(infos, true);
				this.servers.Remove(delegate(ServerEndPoint endpoint)
				{
					foreach (ServersManager<C>.EndpointInfo current in infos)
					{
						if (current.ServerEndPoint.Equals(endpoint))
						{
							return false;
						}
					}
					return true;
				}, new Action<Server<C>>(this.OnServerRemoved));
			}
		}

		private IEnumerable<ServersManager<C>.EndpointInfo> GetEndpointInfos(IEnumerable<ProtocolPort> pps)
		{
			foreach (UnicastIPAddressInformation current in this.NetworkAddresses)
			{
				foreach (ProtocolPort current2 in pps)
				{
					yield return new ServersManager<C>.EndpointInfo(current2, current);
				}
			}
			yield break;
		}

		private IEnumerable<ServersManager<C>.EndpointInfo> GetEndpointInfos(ProtocolPort pp)
		{
			foreach (UnicastIPAddressInformation current in this.NetworkAddresses)
			{
				yield return new ServersManager<C>.EndpointInfo(pp, current);
			}
			yield break;
		}

		private SocketError AddServers(IEnumerable<ServersManager<C>.EndpointInfo> infos, bool ignoreErrors)
		{
			SocketError socketError = SocketError.Success;
			List<Server<C>> list = new List<Server<C>>();
			foreach (ServersManager<C>.EndpointInfo current in infos)
			{
				if (!this.servers.ContainsKey(current.ServerEndPoint))
				{
					IPEndPoint ip4fake = null;
					if (current.ServerEndPoint.AddressFamily == AddressFamily.InterNetwork && current.AddressInformation.IPv4Mask != null)
					{
						ip4fake = this.FakeAddressAction(current.ServerEndPoint);
					}
					Server<C> server = Server<C>.Create(current.ServerEndPoint, ip4fake, current.AddressInformation.IPv4Mask, this.config);
					server.Received = new ServerEventHandlerRef<Server<C>, C, ServerAsyncEventArgs, bool>(this.Server_Received);
					server.Sent = new ServerEventHandlerVal<Server<C>, ServerAsyncEventArgs>(this.Server_Sent);
					server.Failed = new ServerEventHandlerVal<Server<C>, ServerInfoEventArgs>(this.Server_Failed);
					server.NewConnection = new ServerEventHandlerVal<Server<C>, C>(this.Server_NewConnection);
					server.EndConnection = new ServerEventHandlerVal<Server<C>, C>(this.Server_EndConnection);
					server.BeforeSend = new ServerEventHandlerVal<Server<C>, C, ServerAsyncEventArgs>(this.Server_BeforeSend);
					try
					{
						server.Start();
					}
					catch (SocketException ex)
					{
						if (!ignoreErrors)
						{
							socketError = ex.SocketErrorCode;
							break;
						}
						this.OnServerInfo(new ServerInfoEventArgs(current.ServerEndPoint, ex));
					}
					list.Add(server);
				}
			}
			if (!ignoreErrors && list.Count == 0)
			{
				socketError = SocketError.SystemNotReady;
			}
			if (socketError != SocketError.Success)
			{
				using (List<Server<C>>.Enumerator enumerator2 = list.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Server<C> current2 = enumerator2.Current;
						current2.Dispose();
					}
					return socketError;
				}
			}
			foreach (Server<C> current3 in list)
			{
				this.servers.Add(current3.LocalEndPoint, current3);
				this.OnServerAdded(current3);
			}
			return socketError;
		}

		private bool Server_Received(Server<C> server, C c, ref ServerAsyncEventArgs e)
		{
			bool result;
			try
			{
				if (this.logger.IsEnabled)
				{
					this.logger.Write(e, true);
				}
				if (this.Received != null)
				{
					result = this.Received(this, c, ref e);
				}
				else
				{
					result = false;
				}
			}
			catch (Exception innerException)
			{
				throw new Exception("Error in Received event handler", innerException);
			}
			return result;
		}

		private void Server_Sent(Server<C> server, ServerAsyncEventArgs e)
		{
			try
			{
				if (this.Sent != null)
				{
					this.Sent(this, ref e);
				}
				if (e != null)
				{
					EventArgsManager.Put(e);
				}
			}
			catch (Exception innerException)
			{
				throw new Exception("Error in Sent event handler", innerException);
			}
		}

		private void Server_Failed(Server<C> server, ServerInfoEventArgs e)
		{
			this.servers.Remove(server.LocalEndPoint, server);
			this.OnServerRemoved(server);
			this.OnServerInfo(e);
		}

		private void Server_NewConnection(Server<C> server, C e)
		{
			try
			{
				if (this.NewConnection != null)
				{
					this.NewConnection(this, e);
				}
			}
			catch (Exception innerException)
			{
				throw new Exception("Error in NewConnection event handler", innerException);
			}
		}

		private void Server_EndConnection(Server<C> server, C c)
		{
			try
			{
				if (this.EndConnection != null)
				{
					this.EndConnection(this, c);
				}
			}
			catch (Exception innerException)
			{
				throw new Exception("Error in EndConnection event handler", innerException);
			}
		}

		private void Server_BeforeSend(Server<C> server, C c, ServerAsyncEventArgs e)
		{
			try
			{
				if (this.BeforeSend != null)
				{
					this.BeforeSend(this, c, e);
				}
			}
			catch (Exception innerException)
			{
				throw new Exception("Error in BeforeSend event handler", innerException);
			}
		}

		private void OnServerAdded(Server<C> server)
		{
			try
			{
				if (server.FakeEndPoint != null)
				{
					this.fakeServers.Add(server.FakeEndPoint, server);
					if (this.ServerAdded != null)
					{
						this.ServerAdded(this, new ServerChangeEventArgs(server.FakeEndPoint));
					}
				}
				if (this.ServerAdded != null)
				{
					this.ServerAdded(this, new ServerChangeEventArgs(server.LocalEndPoint));
				}
			}
			catch (Exception innerException)
			{
				throw new Exception("Error in ServerAdded event handler", innerException);
			}
		}

		private void OnServerRemoved(Server<C> server)
		{
			server.Dispose();
			try
			{
				if (server.FakeEndPoint != null)
				{
					this.fakeServers.Remove(server.FakeEndPoint, server);
					if (this.ServerRemoved != null)
					{
						this.ServerRemoved(this, new ServerChangeEventArgs(server.FakeEndPoint));
					}
				}
				if (this.ServerRemoved != null)
				{
					this.ServerRemoved(this, new ServerChangeEventArgs(server.LocalEndPoint));
				}
			}
			catch (Exception innerException)
			{
				throw new Exception("Error in ServerRemoved event handler", innerException);
			}
		}

		private void OnServerInfo(ServerInfoEventArgs e)
		{
			try
			{
				if (this.ServerInfo != null)
				{
					this.ServerInfo(this, e);
				}
			}
			catch (Exception innerException)
			{
				throw new Exception("Error in ServerInfo event handler", innerException);
			}
		}
	}
}
