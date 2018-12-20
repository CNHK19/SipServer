using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Pcap
{
	public class PcapWriter : IDisposable
	{
		private enum EtherType : ushort
		{
			None = 65535,
			IPv4 = 2048,
			IPv6 = 34525
		}

		public const int IPv4HeaderLength = 20;

		public const int IPv6HeaderLength = 40;

		public const int EthernetLength = 14;

		public const int UdpLength = 8;

		public const int TcpLength = 20;

		public const int TlsLength = 5;

		public const int MaxRecordLength = 65535;

		private readonly object sync;

		private readonly Stream stream;

		private readonly DateTime nixTimeStart;

		private readonly byte[] mac1;

		private readonly byte[] mac2;

		[ThreadStatic]
		private static MemoryStream cacheStream;

		[ThreadStatic]
		private static BinaryWriter writter;

		public PcapWriter(Stream stream)
		{
			this.sync = new object();
			this.nixTimeStart = new DateTime(1970, 1, 1);
			this.mac1 = new byte[]
			{
				128,
				128,
				128,
				128,
				128,
				128
			};
			this.mac2 = new byte[]
			{
				144,
				144,
				144,
				144,
				144,
				144
			};
			this.stream = stream;
			this.CreateWritter();
			this.WriteGlobalHeader();
			this.WriteChangesToStream();
		}

		public void Dispose()
		{
			this.stream.Dispose();
		}

		public void Flush()
		{
			this.stream.Flush();
		}

		public void WriteComment(string comment)
		{
			this.CreateWritter();
			byte[] bytes = Encoding.UTF8.GetBytes(comment);
			this.WritePacketHeader(bytes.Length + 14);
			this.WriteEthernetHeader(PcapWriter.EtherType.None);
			PcapWriter.writter.Write(bytes);
			this.WriteChangesToStream();
		}

		public void Write(byte[] bytes, Protocol protocol, IPEndPoint source, IPEndPoint destination)
		{
			this.Write(bytes, 0, bytes.Length, protocol, source, destination);
		}

		public void Write(byte[] bytes, int offset, int length, Protocol protocol, IPEndPoint source, IPEndPoint destination)
		{
			this.CreateWritter();
			if (source.AddressFamily != destination.AddressFamily)
			{
				throw new ArgumentException("source.AddressFamily != destination.AddressFamily");
			}
			if (length > 65279)
			{
				length = 65279;
			}
			int num = (protocol == Protocol.Udp) ? 8 : (20 + ((protocol == Protocol.Tls) ? 5 : 0));
			if (source.AddressFamily == AddressFamily.InterNetwork)
			{
				this.WritePacketHeader(length + num + 20 + 14);
				this.WriteEthernetHeader(PcapWriter.EtherType.IPv4);
				this.WriteIpV4Header(length + num, protocol != Protocol.Udp, source.Address, destination.Address);
			}
			else
			{
				if (source.AddressFamily != AddressFamily.InterNetworkV6)
				{
					throw new ArgumentOutOfRangeException("source.AddressFamily");
				}
				this.WritePacketHeader(length + num + 40 + 14);
				this.WriteEthernetHeader(PcapWriter.EtherType.IPv6);
				this.WriteIpV6Header(length + num, protocol != Protocol.Udp, source.Address, destination.Address);
			}
			if (protocol == Protocol.Udp)
			{
				this.WriteUdpHeader(length, (short)source.Port, (short)destination.Port);
			}
			else
			{
				this.WriteTcpHeader(length + ((protocol == Protocol.Tls) ? 5 : 0), (short)source.Port, (short)destination.Port);
				if (protocol == Protocol.Tls)
				{
					this.WriteTlsHeader(length);
				}
			}
			PcapWriter.writter.Write(bytes, offset, length);
			this.WriteChangesToStream();
		}

		private void CreateWritter()
		{
			if (PcapWriter.writter == null)
			{
				PcapWriter.cacheStream = new MemoryStream();
				PcapWriter.writter = new BinaryWriter(PcapWriter.cacheStream);
			}
		}

		private void WriteChangesToStream()
		{
			PcapWriter.cacheStream.Flush();
			lock (this.sync)
			{
				this.stream.Write(PcapWriter.cacheStream.GetBuffer(), 0, Math.Min((int)PcapWriter.cacheStream.Length, 65535));
			}
			PcapWriter.cacheStream.SetLength(0L);
		}

		private void WriteGlobalHeader()
		{
			PcapWriter.writter.Write(2712847316u);
			PcapWriter.writter.Write(2);
			PcapWriter.writter.Write(4);
			PcapWriter.writter.Write(0);
			PcapWriter.writter.Write(0);
			PcapWriter.writter.Write(65535);
			PcapWriter.writter.Write(1);
		}

		private void WritePacketHeader(int length)
		{
			TimeSpan timeSpan = DateTime.UtcNow - this.nixTimeStart;
			PcapWriter.writter.Write((int)timeSpan.TotalSeconds);
			PcapWriter.writter.Write(timeSpan.Milliseconds);
			PcapWriter.writter.Write(length);
			PcapWriter.writter.Write(length);
		}

		private void WriteEthernetHeader(PcapWriter.EtherType etherType)
		{
			PcapWriter.writter.Write(this.mac1);
			PcapWriter.writter.Write(this.mac2);
			PcapWriter.writter.Write(IPAddress.HostToNetworkOrder((short)etherType));
		}

		private void WriteIpV4Header(int length, bool tcpUdp, IPAddress source, IPAddress destination)
		{
			PcapWriter.writter.Write(5);
			PcapWriter.writter.Write(IPAddress.HostToNetworkOrder((short)(length + 20)));
			PcapWriter.writter.Write(0);
			PcapWriter.writter.Write(255);
			PcapWriter.writter.Write(tcpUdp ? 6 : 17);
			PcapWriter.writter.Write(0);
			PcapWriter.writter.Write((int)source.Address);
			PcapWriter.writter.Write((int)destination.Address);
		}

		private void WriteIpV6Header(int length, bool tcpUdp, IPAddress source, IPAddress destination)
		{
			PcapWriter.writter.Write(96);
			PcapWriter.writter.Write(IPAddress.HostToNetworkOrder((short)length));
			PcapWriter.writter.Write(tcpUdp ? 6 : 17);
			PcapWriter.writter.Write(255);
			PcapWriter.writter.Write(source.GetAddressBytes());
			PcapWriter.writter.Write(destination.GetAddressBytes());
		}

		private void WriteUdpHeader(int length, short sourcePort, short destinationPort)
		{
			PcapWriter.writter.Write(IPAddress.HostToNetworkOrder(sourcePort));
			PcapWriter.writter.Write(IPAddress.HostToNetworkOrder(destinationPort));
			PcapWriter.writter.Write(IPAddress.HostToNetworkOrder((short)(8 + length)));
			PcapWriter.writter.Write(0);
		}

		private void WriteTcpHeader(int length, short sourcePort, short destinationPort)
		{
			PcapWriter.writter.Write(IPAddress.HostToNetworkOrder(sourcePort));
			PcapWriter.writter.Write(IPAddress.HostToNetworkOrder(destinationPort));
			PcapWriter.writter.Write(IPAddress.HostToNetworkOrder(0));
			PcapWriter.writter.Write(IPAddress.HostToNetworkOrder(0));
			PcapWriter.writter.Write(80);
			PcapWriter.writter.Write(2);
			PcapWriter.writter.Write(16383);
			PcapWriter.writter.Write(0);
			PcapWriter.writter.Write(0);
		}

		private void WriteTlsHeader(int length)
		{
			PcapWriter.writter.Write(23);
			PcapWriter.writter.Write(259);
			PcapWriter.writter.Write(IPAddress.HostToNetworkOrder((short)length));
		}
	}
}
