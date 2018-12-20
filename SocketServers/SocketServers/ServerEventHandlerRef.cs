using System;

namespace SocketServers
{
	public delegate void ServerEventHandlerRef<S, T>(S s, ref T e);
	public delegate R ServerEventHandlerRef<S, T1, T2, R>(S s, T1 t1, ref T2 t2);
}
