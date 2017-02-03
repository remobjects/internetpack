/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
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

		public Boolean IsSuperUser { get; set; }

		public Boolean IsFileAdmin { get; set; }

		public IFtpFolder CurrentFolder { get; set; }
	}
}
