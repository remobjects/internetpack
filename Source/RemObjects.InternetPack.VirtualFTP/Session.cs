/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET - Virtual FTP Library
  (c)opyright RemObjects Software, LLC. 2003-2012. All rights reserved.

  Using this code requires a valid license of the RemObjects Internet Pack
  which can be obtained at http://www.remobjects.com?ip.
---------------------------------------------------------------------------*/

using System;

namespace RemObjects.InternetPack.Ftp.VirtualFtp
{
    public class VirtualFtpSession : FtpSession
    {
        public VirtualFtpSession()
        {
        }

        public override String Directory
        {
            get
            {
                return CurrentFolder.FullPath;
            }
            set
            {
                base.Directory = value;
            }
        }

        public Boolean IsSuperUser
        {
            get
            {
                return fIsSuperUser;
            }
            set
            {
                fIsSuperUser = value;
            }
        }
        private Boolean fIsSuperUser;

        public Boolean IsFileAdmin
        {
            get
            {
                return fIsFileAdmin;
            }
            set
            {
                fIsFileAdmin = value;
            }
        }
        private Boolean fIsFileAdmin;

        public IFtpFolder CurrentFolder
        {
            get
            {
                return fCurrentFolder;
            }
            set
            {
                fCurrentFolder = value;
            }
        }
        private IFtpFolder fCurrentFolder;
    }
}
