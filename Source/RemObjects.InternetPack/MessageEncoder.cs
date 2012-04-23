/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET - Core Library
  (c)opyright RemObjects Software, LLC. 2003-2012. All rights reserved.

  Using this code requires a valid license of the RemObjects Internet Pack
  which can be obtained at http://www.remobjects.com?ip.
---------------------------------------------------------------------------*/

using System.IO;

namespace RemObjects.InternetPack.Messages
{
    public abstract class MessageEncoder
    {
        public abstract void EncodeMessage(MailMessage source, Stream destination);
        public abstract void DecodeMessage(MailMessage destination, Stream source);
    }
}
