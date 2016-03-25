/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.IO;

namespace RemObjects.InternetPack.Messages
{
	public class MessageAttachment : IDisposable
	{
		#region Properties
		public Boolean OwnsData { get; set; }

		public Stream Data { get; set; }

		public String FileName { get; set; }
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
		private readonly IList<MessageAttachment> fItems = new List<MessageAttachment>(8);

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
			{
				if (item != null)
				{
					((IDisposable)item).Dispose();
				}
			}

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
			item.FileName = Name;

			fItems.Add(item);

			return item;
		}

		public MessageAttachment Add(String name, Stream data, Boolean ownsData)
		{
			MessageAttachment item = new MessageAttachment();
			item.FileName = name;
			item.Data = data;
			item.OwnsData = ownsData;

			fItems.Add(item);

			return item;
		}
	}
}