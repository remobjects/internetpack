/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET - Virtual FTP Library
  (c)opyright RemObjects Software, LLC. 2003-2012. All rights reserved.

  Using this code requires a valid license of the RemObjects Internet Pack
  which can be obtained at http://www.remobjects.com?ip.
---------------------------------------------------------------------------*/

using System;
using System.IO;

namespace RemObjects.InternetPack.Ftp.VirtualFtp
{
    public class EmptyFile : FtpFile
    {
        public EmptyFile(IFtpFolder parent, String name)
            : base(parent, name)
        {
            this.UserRead = false;
            this.UserWrite = false;
            this.Complete = false;
        }

        public override Int32 Size
        {
            get
            {
                return 0;
            }
            set
            {
                /* no-op */
            }
        }

        public override void GetFile(Stream destination)
        {
            /* no-op */
        }

        public override void CreateFile(Stream source)
        {
            /* no-op */
        }
    }
}