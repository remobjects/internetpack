/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

using System.IO;

namespace RemObjects.InternetPack.Messages
{
	public interface IMessageEncoder
	{
		void EncodeMessage(MailMessage source, Stream destination);
		void DecodeMessage(MailMessage destination, Stream source);
	}
}
