/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET - Core Library
  (c)opyright RemObjects Software, Inc. 2003-2012. All rights reserved.

  Using this code requires a valid license of the RemObjects Internet Pack
  which can be obtained at http://www.remobjects.com?ip.
---------------------------------------------------------------------------*/

using System;
using System.Collections;

namespace RemObjects.InternetPack
{
    public class WorkerCollection : ArrayList
    {
        private void OnDone(Object sender, EventArgs e)
        {
            lock (this)
            {
                base.Remove(sender);
            }
        }

        public void Add(IWorker worker)
        {
            worker.Done += new EventHandler(OnDone);
            base.Add(worker);
        }

        public void Remove(IWorker worker)
        {
            worker.Done -= new EventHandler(OnDone);
            base.Remove(worker);
        }

        public void Close()
        {
#if FULLFRAMEWORK
            Object[] lWaitArray;
#endif
            lock (this)
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
