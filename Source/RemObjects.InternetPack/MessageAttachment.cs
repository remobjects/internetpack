/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET - Core Library
  (c)opyright RemObjects Software, LLC. 2003-2012. All rights reserved.

  Using this code requires a valid license of the RemObjects Internet Pack
  which can be obtained at http://www.remobjects.com?ip.
---------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.IO;

namespace RemObjects.InternetPack.Messages
{
    public class MessageAttachment : IDisposable
    {
        #region Properties
        public Boolean OwnsData
        {
            get
            {
                return this.fOwnsData;
            }
            set
            {
                this.fOwnsData = value;
            }
        }
        private Boolean fOwnsData;

        public Stream Data
        {
            get
            {
                return this.fData;
            }
            set
            {
                this.fData = value;
            }
        }
        private Stream fData;

        public String Filename
        {
            get
            {
                return this.fFilename;
            }
            set
            {
                this.fFilename = value;
            }
        }
        private String fFilename;
        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            if (this.OwnsData && (this.Data != null))
                this.Data.Close();
        }
        #endregion
    }

    public class MessageAttachments
    {
        private IList<MessageAttachment> fItems = new List<MessageAttachment>(8);

        public Int32 Count
        {
            get
            {
                return this.fItems.Count;
            }
        }

        public MessageAttachment this[Int32 index]
        {
            get
            {
                return this.fItems[index];
            }
        }

        public void Clear()
        {
            foreach (MessageAttachment item in this.fItems)
                if (item is IDisposable)
                    ((IDisposable)item).Dispose();

            this.fItems.Clear();
        }

        public MessageAttachment Add()
        {
            MessageAttachment item = new MessageAttachment();
            this.fItems.Add(item);

            return item;
        }

        public MessageAttachment Add(String Name)
        {
            MessageAttachment item = new MessageAttachment();
            item.Filename = Name;

            fItems.Add(item);

            return item;
        }

        public MessageAttachment Add(String name, Stream data, Boolean ownsData)
        {
            MessageAttachment item = new MessageAttachment();
            item.Filename = name;
            item.Data = data;
            item.OwnsData = ownsData;

            fItems.Add(item);

            return item;
        }
    }
}