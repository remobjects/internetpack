using System;
using System.IO;

namespace RemObjects.InternetPack.Ftp.VirtualFtp
{
  
  public abstract class FtpFile : FtpItem, IFtpFile
	{
    protected FtpFile(IFtpFolder aParent, string aName) : base(aParent, aName)
		{
		}
        /* ToDo: add file rights */
    public abstract void GetFile(Stream aToStream);    public abstract void CreateFile(Stream aStream);  
    private bool fComplete;    public bool Complete    {      get { return fComplete; }      set	{ fComplete = value; }    }

    public override void FillFtpListingItem(FtpListingItem aItem)
    {
      base.FillFtpListingItem(aItem);
      aItem.Directory = false;
      aItem.Size = Size;
      aItem.UserRead = Complete && UserRead;
      aItem.UserWrite = Complete && UserWrite;
      aItem.UserExec = false;
      aItem.GroupRead = Complete && GroupRead;
      aItem.GroupWrite = Complete && GroupWrite;
      aItem.GroupExec = false;
      aItem.OtherRead = Complete && WorldRead;
      aItem.OtherWrite = Complete && WorldWrite;
      aItem.OtherExec = false;
    }


    public virtual bool AllowGet(VirtualFtpSession aSession)    {      return Complete && AllowRead(aSession);
    }    public virtual bool AllowAppend(VirtualFtpSession aSession)    {      return Complete && AllowWrite(aSession);
    }    public virtual bool AllowDelete(VirtualFtpSession aSession)    {      return Complete && AllowWrite(aSession);
    }    public virtual bool AllowRename(VirtualFtpSession aSession)    {      return Complete && AllowWrite(aSession);
    }  }

}
