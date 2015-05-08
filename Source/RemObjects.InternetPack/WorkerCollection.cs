/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, Inc. 2003-2015. All rights reserved.
---------------------------------------------------------------------------*/

using System;
using System.Collections;

namespace RemObjects.InternetPack
{
	public class WorkerCollection : ArrayList
	{
		#region Private fields

		private readonly Object fSyncRoot;
		#endregion

		public WorkerCollection()
		{
			this.fSyncRoot = new Object();
		}

		private void OnDone(Object sender, EventArgs e)
		{
			lock (this.fSyncRoot)
			{
				base.Remove(sender);
			}
		}

		public void Add(IWorker worker)
		{
			lock (this.fSyncRoot)
			{
				worker.Done += OnDone;
				base.Add(worker);
			}
		}

		public void Remove(IWorker worker)
		{
			lock (this.fSyncRoot)
			{
				worker.Done -= OnDone;
				base.Remove(worker);
			}
		}

		public void Close()
		{
#if FULLFRAMEWORK
			Object[] lWaitArray;
#endif
			lock (this.fSyncRoot)
			{
				foreach (IWorker worker in this)
					worker.DataConnection.Close();

#if FULLFRAMEWORK
				lWaitArray = new Object[Count];
				this.CopyTo(lWaitArray);
#endif
			}

#if FULLFRAMEWORK
			foreach (IWorker worker in lWaitArray)
				worker.Thread.Join();
#endif
		}
	}
}