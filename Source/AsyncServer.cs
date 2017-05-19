/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/
#if macos || ios
using RemObjects.Elements.RTL.Reflection;
#endif

namespace RemObjects.InternetPack
{
	public abstract class AsyncServer : Server
	{
		private List<IAsyncWorker> fWorkers;
		#if macos || ios
        private readonly Object fSyncRoot = new Object();        
        #else
        private readonly Monitor fSyncRoot = new Monitor();
        #endif

		public override void Open()
		{
			try
			{
#if FULLFRAMEWORK
				if (this.SslOptions.Enabled)
				{
					this.SslOptions.LoadCertificate();
				}
#endif

				this.fWorkers = CloseConnectionsOnShutdown ? new List<IAsyncWorker>() : null;

				Int32 lActualPort = this.Port;

				Boolean lBindV6 = (this.BindingV6 != null) && this.BindV6;

				if (lBindV6)
				{
					this.BindingV6.BindUnthreaded();
					this.BindingV6.ListeningSocket.Listen(this.BindingV6.MaxWaitConnections);
					this.BindingV6.ListeningSocket.BeginAccept(AcceptCallback, this.BindingV6.ListeningSocket);
					lActualPort = ((IPEndPoint)this.BindingV6.ListeningSocket.LocalEndPoint).Port;
				}

				// There is a chance that this will fail on Mono
				// Unfortunately this code shouldn't fail on Mac while it WILL fail on Linux
				// And no one can warrant that suddenly the Mono/Linux issue won't be fixed
				if (this.BindingV4 != null && this.BindV4)
				{
					try
					{
						this.BindingV4.BindUnthreaded();
						this.BindingV4.ListeningSocket.Listen(this.BindingV4.MaxWaitConnections);
						this.BindingV4.ListeningSocket.BeginAccept(AcceptCallback, this.BindingV4.ListeningSocket);
						lActualPort = ((IPEndPoint)this.BindingV4.ListeningSocket.LocalEndPoint).Port;
					}
					catch (SocketException)
					{
						if (!(lBindV6 && Environment.IsMono))
							throw;
					}
				}

				this.Port = lActualPort;

				this.fActive = true;
			}
			catch (Exception)
			{
				this.Close();
				throw;
			}
		}

		private void AcceptCallback(IAsyncResult ar)
		{
			try
			{
				Socket lSocket;
				try
				{
					lSocket = ((Socket)ar.AsyncState).EndAccept(ar);
				}
				catch (ObjectDisposedException)
				{
					return; // sometimes the object is gc'd before we even get here.
				}
				catch (SocketException)
				{
					return;
				}

				#if !NEEDS_PORTING
				Object lObject = Activator.CreateInstance(this.GetWorkerClass());
				#else
				Object lObject = null;
				#endif

				IAsyncWorker lWorker = (IAsyncWorker)lObject;
				lWorker.Owner = this;

				if (this.ConnectionFactory != null)
				{
					lWorker.DataConnection = ConnectionFactory.CreateServerConnection(lSocket);
				}
				#if ECHOES
				else if (this.ConnectionClass != null)
				{
					lWorker.DataConnection = (Connection)Activator.CreateInstance(ConnectionClass);
					lWorker.DataConnection.Init(lSocket);
				}
				#endif
				#if FULLFRAMEWORK
				else if (this.SslOptions.Enabled)
				{
					lWorker.DataConnection = this.SslOptions.CreateServerConnection(lSocket);
				}
				#endif
				else
				{
					lWorker.DataConnection = new Connection(lSocket);
				}

#if FULLFRAMEWORK
				if (this.TimeoutEnabled)
				{
					lWorker.DataConnection.TimeoutEnabled = true;
					lWorker.DataConnection.Timeout = this.Timeout;
				}
#endif
				if (this.MaxLineLengthEnabled)
				{
					lWorker.DataConnection.MaxLineLengthEnabled = true;
					lWorker.DataConnection.MaxLineLength = this.MaxLineLength;
				}

				if (this.fWorkers != null)
				{
					lock (this.fSyncRoot)
					{
						this.fWorkers.Add(lWorker);
					}
				}

				if (lWorker.DataConnection.BeginInitializeServerConnection(SetupCallback, lWorker) == null)
					lWorker.Setup();
			}
			catch
			{
				// Is not a perfect solution but we must not allow exceptions to escape
			}
			try
			{
				((Socket)ar.AsyncState).BeginAccept(AcceptCallback, ar.AsyncState);
			}
			catch
			{
				// Is not a perfect solution but we must not allow exceptions to escape
			}
		}

		private void SetupCallback(IAsyncResult ar)
		{
			try
			{
				((IAsyncWorker)ar.AsyncState).DataConnection.EndInitializeServerConnection(ar);
			}
			catch (Exception) // we are running async; if something escapes here the whole app goes down.
			{
				((IAsyncWorker)ar.AsyncState).DataConnection.Dispose();
				return;
			}

			((IAsyncWorker)ar.AsyncState).Setup();
		}

		public override void Close()
		{
			try
			{
				if (this.BindingV4 != null && this.BindV4)
					this.BindingV4.Unbind(false);

				if (this.BindingV6 != null && this.BindV6)
					this.BindingV6.Unbind(false);
			}
			catch (Exception)
			{
				// avoid GC/Socket errors
			}

			if (this.fWorkers != null)
			{
				lock (this.fSyncRoot)
				{
					for (Int32 i = this.fWorkers.Count - 1; i >= 0; i--)
					{
						try
						{
							((IAsyncWorker)this.fWorkers[i]).DataConnection.Close();
						}
						catch (SocketException)
						{
						}
						catch (ObjectDisposedException)
						{
						}
					}
				}
			}
			this.fActive = false;
		}

		public override Type GetWorkerClass()
		{
			return typeof(AsyncWorker);
		}

		public void ClientClosed(IAsyncWorker worker)
		{
			if (this.fWorkers == null)
				return;

			lock (this.fSyncRoot)
			{
				this.fWorkers.Remove(worker);
			}
		}
	}
}