/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

namespace RemObjects.InternetPack
{
	public class SimpleServer
	{
		public SimpleServer()
		{
			this.fBinding = new ServerBinding();
			this.fBinding.Address = IPAddress.Any;
		}

		#region Properties
		public ServerBinding Binding
		{
			get
			{
				return this.fBinding;
			}
		}
		private readonly ServerBinding fBinding;
		#endregion

		public void Open()
		{
			this.Binding.BindUnthreaded();
			this.Binding.ListeningSocket.Listen(this.Binding.MaxWaitConnections);
		}

		public void Close()
		{
			this.Binding.Unbind();
		}

		public Connection WaitForConnection()
		{
			return this.Binding.Accept();
		}
	}
}