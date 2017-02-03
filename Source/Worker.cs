/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

namespace RemObjects.InternetPack
{
	public abstract class AsyncWorker : IAsyncWorker
	{
		public AsyncServer AsyncOwner { get; set; }

		public virtual Server Owner
		{
			get
			{
				return this.AsyncOwner;
			}
			set
			{
				this.AsyncOwner = (AsyncServer)value;
			}
		}

		public Connection DataConnection { get; set; }

		public abstract void Setup();

		public virtual void Done()
		{
			if (this.AsyncOwner != null)
			{
				this.AsyncOwner.ClientClosed(this);
			}

			if (this.DataConnection != null)
			{
				this.DataConnection.Dispose();
			}
		}
	}

	public abstract class Worker : IWorker
	{
		protected Worker()
		{
		}

		public Connection DataConnection { get; set; }

		public Thread Thread { get; set; }

		public Server Owner { get; set; }

		protected abstract void DoWork();

		public event EventHandler Done;

		public void Work()
		{
			try
			{
				this.DoWork();
			}
			catch (Exception)
			{
				// As designed
			}
			finally
			{
				this.DataConnection.Close();

				if (this.Done != null)
					this.Done(this, EventArgs.Empty);
			}
		}
	}
}