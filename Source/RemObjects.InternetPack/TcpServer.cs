/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

namespace RemObjects.InternetPack
{
#if DESIGN
	[System.Drawing.ToolboxBitmap(typeof(RemObjects.InternetPack.Server), "Glyphs.TcpServer.bmp")]
#endif
	public class TcpServer : Server
	{
		public TcpServer()
		{
		}

#if FULLFRAMEWORK
		public TcpServer(IContainer container)
			: this()
		{
			container.Add(this);
		}
#endif

		public override Type GetWorkerClass()
		{
			return typeof(TcpWorker);
		}

		#region Events
		public event OnTcpConnectionHandler OnTcpConnection;

		protected virtual void TriggerOnTcpConnection(Connection connection)
		{
			if (this.OnTcpConnection != null)
			{
				OnTcpConnectionArgs lEventArgs = new OnTcpConnectionArgs(connection);
				this.OnTcpConnection(this, lEventArgs);
			}
		}
		#endregion

		#region Methods for implementation in descendand classes
		internal protected virtual void HandleTcpConnection(Connection connection)
		{
			this.TriggerOnTcpConnection(connection);
		}
		#endregion
	}

	public class TcpWorker : Worker
	{
		public TcpWorker()
		{
		}

		public new TcpServer Owner
		{
			get
			{
				return (TcpServer)base.Owner;
			}
		}

		protected override void DoWork()
		{
			try
			{
				Owner.HandleTcpConnection(DataConnection);
			}
			catch (SocketException ex)
			{
				// 10054 means the connection was closed by the client while reading from the socket.
				// We'll just terminate the thread gracefully, as if this was expected.
				if (ex.ErrorCode != 10054)
				{
					throw;
				}
			}
			catch (Exception)
			{
				// Ignore all exceptions
			}
			finally
			{
				DataConnection.Close();
			}
		}
	}

	#region Events
	public delegate void OnTcpConnectionHandler(Object sender, OnTcpConnectionArgs e);

	public class OnTcpConnectionArgs : ConnectionEventArgs
	{
		public OnTcpConnectionArgs(Connection connection)
			: base(connection)
		{
		}
	}
	#endregion
}