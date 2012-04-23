using System;
using System.IO;

namespace RemObjects.InternetPack.Ftp.VirtualFtp
{
	public class EmptyFile : FtpFile
	{
		public EmptyFile(IFtpFolder aParent, string aName) : base(aParent, aName)
		{
      UserRead = false;
      UserWrite = false;
      Complete = false;
    }

    public override int Size { get { return 0; } set { /* no-op */ } }

    public override void GetFile(Stream aToStream)    {      /* no-op */    }    public override void CreateFile(Stream aStream)    {      /* no-op */    }
	}
}
