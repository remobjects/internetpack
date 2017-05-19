/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

namespace RemObjects.InternetPack
{
	public class WorkerCollection
	{
		#region Private fields

    	#if macos
        private readonly Object fSyncRoot = new Object();        
        #else
        private readonly Monitor fSyncRoot = new Monitor();
        #endif
		private readonly List<IWorker> fWorkers = new List<IWorker>();
		#endregion

		public WorkerCollection()
		{
		}

		private void OnDone(Object sender, EventArgs e)
		{
			lock (fSyncRoot)
			{
				fWorkers.Remove(sender as IWorker);
			}
		}

		public void Add(IWorker worker)
		{
			lock (fSyncRoot)
			{
				worker.Done += OnDone;
				fWorkers.Add(worker);
			}
		}

		public void Remove(IWorker worker)
		{
			lock (fSyncRoot)
			{
				worker.Done -= OnDone;
				fWorkers.Remove(worker);
			}
		}

		public void Close()
		{
			#if FULLFRAMEWORK
			Object[] lWaitArray;
			#endif
			lock (fSyncRoot)
			{
				foreach (IWorker worker in fWorkers)
					worker.DataConnection.Close();

				#if FULLFRAMEWORK
				lWaitArray = fWorkers.ToArray();
				#endif
			}

			#if FULLFRAMEWORK
			foreach (IWorker worker in lWaitArray)
				worker.Thread.Join();
			#endif
		}
	}
}