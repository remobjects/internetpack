/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Text;

namespace RemObjects.InternetPack.Messages
{
	public class MessageAddress
	{
		public String Name { get; set; } = String.Empty;

		public String Address { get; set; } = String.Empty;

		public void Clear()
		{
			this.Name = "";
			this.Address = "";
		}

		public Boolean IsSet()
		{
			return (this.Name.Length != 0) || (this.Address.Length != 0);
		}

		public override String ToString()
		{
			return String.Format("{0} <{1}>", this.Name, this.Address);
		}

		public void FromString(String input)
		{
			Int32 i = input.IndexOf('<');
			if (i == -1)
			{
				this.Address = input;
				this.Name = "";
			}
			else
			{
				this.Name = input.Substring(0, i).Trim();
				input = input.Substring(i + 1);
				i = input.IndexOf('>');

				if (i != -1)
					input = input.Substring(0, i);

				this.Address = input;
			}
		}

		public static MessageAddress ParseAddress(String address)
		{
			MessageAddress lAddress = new MessageAddress();
			lAddress.FromString(address);

			return lAddress;
		}
	}

	public class MessageAddresses : List<MessageAddress>
	{
		public MessageAddress Add(String name, String address)
		{
			MessageAddress item = new MessageAddress();
			item.Name = name;
			item.Address = address;

			this.Add(item);

			return item;
		}

		public MessageAddress Add(String address)
		{
			MessageAddress item = new MessageAddress();
			item.FromString(address);

			this.Add(item);

			return item;
		}

		public override String ToString()
		{
			StringBuilder lResult = new StringBuilder();
			Boolean lIsFirstItem = true;

			foreach (MessageAddress address in this)
			{
				if (!lIsFirstItem)
				{
					lResult.Append(", ");
				}
				else
				{
					lIsFirstItem = false;
				}

				lResult.Append(address);
			}

			return lResult.ToString();
		}

		public static MessageAddresses ParseAddresses(IEnumerable<String> addresses)
		{
			MessageAddresses lAddresses = new MessageAddresses();

			foreach (String address in addresses)
			{
				lAddresses.Add(MessageAddress.ParseAddress(address));
			}

			return lAddresses;
		}
	}
}