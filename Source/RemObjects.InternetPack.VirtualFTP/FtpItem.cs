using System;

namespace RemObjects.InternetPack.Ftp.VirtualFtp
{

  public abstract class FtpItem : IFtpItem
  {
    protected FtpItem(IFtpFolder aParent, string aName)
    {
      fParent = aParent;
      fName = aName;

      UserRead = true;
      UserWrite = true;
      GroupRead = true;
      
      OwningUser = "system";
      OwningGroup = "system";

      Date = DateTime.Now;
    }

    #region General Item attributes
    private IFtpFolder fParent;    public IFtpFolder Parent { get { return fParent; } set { fParent = value; } }
    public IFtpFolder Root    {      get       {         IFtpFolder lParent;        if (this is IFtpFolder)
        {
          lParent = this as IFtpFolder;        }        else        {          lParent = Parent;        }
        while (lParent.Parent != null) lParent = lParent.Parent;        return lParent;      }    }

    private string fName;    public virtual string Name { get { return fName; } set { fName = value; } }
    private DateTime fDate;
    public virtual DateTime Date { get { return fDate; } set { fDate = value; } }
    public abstract int Size { get; set; }    #endregion
    #region Rights
    private string fOwningUser;    public virtual string OwningUser { get { return fOwningUser; } set	{ fOwningUser = value; } }

    private string fOwningGroup;    public virtual string OwningGroup { get { return fOwningGroup; } set	{ fOwningGroup = value; } }

    public virtual bool AllowRead(VirtualFtpSession aSession)    {      if (Invalid) return false;
      if (aSession.IsFileAdmin) return true;
      if (aSession.Username == OwningUser && UserRead) return true;
      if (WorldRead) return true;
      return false;
    }    public virtual bool AllowWrite(VirtualFtpSession aSession)    {      if (Invalid) return false;
      if (aSession.IsFileAdmin) return true;
      if (aSession.Username == OwningUser && UserWrite) return true;
      if (WorldWrite) return true;
      return false;
    }
    private bool[] fRights = new bool[6];    public virtual bool UserRead     { get { return fRights[0]; } set	{ fRights[0] = value; } }    public virtual bool UserWrite    { get { return fRights[1]; } set	{ fRights[1] = value; } }    public virtual bool GroupRead    { get { return fRights[2]; } set	{ fRights[2] = value; } }    public virtual bool GroupWrite   { get { return fRights[3]; } set	{ fRights[3] = value; } }    public virtual bool WorldRead    { get { return fRights[4]; } set	{ fRights[4] = value; } }    public virtual bool WorldWrite   { get { return fRights[5]; } set	{ fRights[5] = value; } }    #endregion

    #region Invalidation
    private bool fInvalid;    public virtual bool Invalid { get { return fInvalid; } set	{ fInvalid = value; } }
    public virtual void Invalidate()    {      Invalid = false;    }    #endregion
    public virtual void FillFtpListingItem(FtpListingItem aItem, string aAsName)
    {
      FillFtpListingItem(aItem);
      aItem.FileName = aAsName;
    }
     
    public virtual void FillFtpListingItem(FtpListingItem aItem)
    {
      aItem.Directory = (this is IFtpFolder);
      aItem.FileName = Name;
      aItem.FileDate = Date;
      aItem.Size = Size;
      aItem.User = OwningUser;
      aItem.Group = OwningGroup;
      aItem.UserRead = UserRead;
      aItem.UserWrite = UserWrite;
      aItem.UserExec = aItem.Directory && UserRead;
      aItem.GroupRead = GroupRead;
      aItem.GroupWrite = GroupWrite;
      aItem.GroupExec = aItem.Directory && GroupRead;
      aItem.OtherRead = WorldRead;
      aItem.OtherWrite = WorldWrite;
      aItem.OtherExec = aItem.Directory && WorldRead;
    }

  }

}
