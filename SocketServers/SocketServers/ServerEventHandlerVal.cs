using System;

namespace SocketServers
{
	public delegate void ServerEventHandlerVal<S, T>(S s, T e);
	public delegate void ServerEventHandlerVal<S, T1, T2>(S s, T1 t1, T2 t2);
}
