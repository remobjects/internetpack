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
    public abstract class FtpFile : FtpItem, IFtpFile
    {
        protected FtpFile(IFtpFolder parent, String name)
            : base(parent, name)
        {
        }

        public abstract void GetFile(Stream destination);

        public abstract void CreateFile(Stream source);

        public Boolean Complete
        {
            get
            {
                return fComplete;
            }
            set
            {
                fComplete = value;
            }
        }
        private Boolean fComplete;

        public override void FillFtpListingItem(FtpListingItem item)
        {
            base.FillFtpListingItem(item);
            item.Directory = false;
            item.Size = Size;
            item.UserRead = Complete && UserRead;
            item.UserWrite = Complete && UserWrite;
            item.UserExec = false;
            item.GroupRead = Complete && GroupRead;
            item.GroupWrite = Complete && GroupWrite;
            item.GroupExec = false;
            item.OtherRead = Complete && WorldRead;
            item.OtherWrite = Complete && WorldWrite;
            item.OtherExec = false;
        }

        public virtual Boolean AllowGet(VirtualFtpSession session)
        {
            return Complete && AllowRead(session);
        }

        public virtual Boolean AllowAppend(VirtualFtpSession session)
        {
            return Complete && AllowWrite(session);
        }

        public virtual Boolean AllowDelete(VirtualFtpSession session)
        {
            return Complete && AllowWrite(session);
        }

        public virtual Boolean AllowRename(VirtualFtpSession session)
        {
            return Complete && AllowWrite(session);
        }
    }
}