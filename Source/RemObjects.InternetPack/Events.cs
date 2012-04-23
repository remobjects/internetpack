/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET - Core Library
  (c)opyright RemObjects Software, LLC. 2003-2012. All rights reserved.

  Using this code requires a valid license of the RemObjects Internet Pack
  which can be obtained at http://www.remobjects.com?ip.
---------------------------------------------------------------------------*/

using System;

namespace RemObjects.InternetPack.Events
{
    public delegate void TransferStartEventHandler(Object sender, TransferStartEventArgs e);
    public delegate void TransferEndEventHandler(Object sender, TransferEndEventArgs e);
    public delegate void TransferProgressEventHandler(Object sender, TransferProgressEventArgs e);

    public enum TransferDirection
    {
        Send,
        Receive
    }

    public class TransferEventArgs
    {
        public TransferEventArgs(TransferDirection direction)
            : base()
        {
            this.fDirection = direction;
        }

        public TransferDirection TransferDirection
        {
            get
            {
                return this.fDirection;
            }
        }
        private TransferDirection fDirection;
    }

    public class TransferStartEventArgs : TransferEventArgs
    {
        public TransferStartEventArgs(TransferDirection direction, Int64 total)
            : base(direction)
        {
            this.fTotal = total;
        }

        public Int64 Total
        {
            get
            {
                return this.fTotal;
            }
        }
        private Int64 fTotal;
    }

    public class TransferEndEventArgs : TransferEventArgs
    {
        public TransferEndEventArgs(TransferDirection direction)
            : base(direction)
        {
        }
    }

    public class TransferProgressEventArgs : TransferEventArgs
    {
        public TransferProgressEventArgs(TransferDirection direction, Int64 current)
            : base(direction)
        {
            this.fCurrent = current;
        }

        public Int64 Current
        {
            get
            {
                return this.fCurrent;
            }
        }
        private Int64 fCurrent;
    }
}