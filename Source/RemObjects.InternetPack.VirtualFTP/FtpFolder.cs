using System;
using System.Text;
using System.Collections;
using System.IO;

namespace RemObjects.InternetPack.Ftp.VirtualFtp
{
  public abstract class FtpFolder : FtpItem, IFtpFolder
  {
    protected FtpFolder(IFtpFolder aParent, string aName) : base(aParent, aName)
    {
      fSubFolderList = new Hashtable();
    }

    public override int Size { get { return 0; } set { /* no-op */ } }
    private Hashtable fSubFolderList;    protected Hashtable SubFolderList    {      get { return fSubFolderList; }    }

    #region IFtpFolder implementation

    public abstract IEnumerable SubFolders { get ; }    public abstract IEnumerable Files { get ; }    public abstract bool HasSubfolder(string aFolderName);    public abstract IFtpFolder GetSubFolder(string aFolderName, VirtualFtpSession aSession);    public abstract IFtpFolder CreateFolder(string aFolderName, VirtualFtpSession aSession);    public abstract void DeleteFolder(string aFolderName, bool aRecursive, VirtualFtpSession aSession);        public abstract bool HasFile(string aFileame);    public abstract IFtpFile GetFile(string aFilename, VirtualFtpSession aSession);    public abstract IFtpFile CreateFile(string aFilename, VirtualFtpSession aSession);    public abstract void DeleteFile(string aFilename, VirtualFtpSession aSession);    public abstract void RenameFileOrFolder(string aOldFilename, string aNewFilename, VirtualFtpSession aSession);    public abstract void RemoveItem(IFtpItem aItem);                   public IFtpFolder DigForSubFolder(string aFullPath, VirtualFtpSession aSession)    {      if (aFullPath.Length == 0) 
        return this;
            int p = aFullPath.IndexOf('/');
      if (p >= 0)
      {
        string lSubFolder = aFullPath.Substring(0,p);

        IFtpFolder lFolder = GetSubFolder(lSubFolder, aSession);

        if (lFolder != null)
        {
          aFullPath = aFullPath.Substring(p+1);
          return lFolder.DigForSubFolder(aFullPath, aSession);
        }  

        else
        {
          return null;
        }
      }
      else       {        return GetSubFolder(aFullPath, aSession);      }    }    public void FindBaseFolderForFilename(string aPath, out IFtpFolder aFolder, out string aFilename, VirtualFtpSession aSession)    {      if (aPath.IndexOf('/') != -1)
      {
        if (aPath.StartsWith("/"))
        {
          aFolder = Root;
          aPath = aPath.Substring(1); /* remove / */
        }
        else
        {
          aFolder = this;
        }
        int p = aPath.IndexOf('/');        while (p >= 0)
        {
          string lFolderName = aPath.Substring(0,p);
          aFolder = aFolder.GetSubFolder(lFolderName, aSession);

          if (aFolder == null || !aFolder.AllowBrowse(aSession))
            throw new FtpException(550, String.Format("Folder \"{0}\" does not exists, or access denied.", aPath));
        
          aPath = aPath.Substring(p+1);
          p = aPath.IndexOf('/');        }
      }
      else
      {
        aFolder = this;
      }

      aFilename = aPath;
    }    public string FullPath     {       get       {
        if (Parent != null)
        {
          return Parent.FullPath+Name+"/";
        }
        else        {          return "/";        }      }    }    public virtual bool AllowBrowse(VirtualFtpSession aSession)    {      return AllowRead(aSession);
    }    public virtual bool AllowGet(VirtualFtpSession aSession)    {      return AllowRead(aSession);
    }    public virtual bool AllowPut(VirtualFtpSession aSession)    {      return AllowWrite(aSession);
    }    public virtual bool AllowMkDir(VirtualFtpSession aSession)    {      return AllowWrite(aSession);
    }
    public virtual bool AllowDeleteItems(VirtualFtpSession aSession)    {      return AllowWrite(aSession);
    }
    public virtual bool AllowRenameItems(VirtualFtpSession aSession)    {      return AllowWrite(aSession);
    }
    public virtual bool AllowDeleteThis(VirtualFtpSession aSession)    {      return AllowWrite(aSession);
    }    #endregion

    #region FolderListing
    public void ListFolderItems(FtpListing aListing)    {      /* Add this folder (.) */
      FtpListingItem lListingItem;
      lListingItem = aListing.Add();
      FillFtpListingItem(lListingItem, ".");
      
      /* Add parent folder (..) */
      if (Parent != null)
      {
        lListingItem = aListing.Add();
        Parent.FillFtpListingItem(lListingItem, "..");
      }

      DoListFolderItems(aListing);
    }    public virtual void DoListFolderItems(FtpListing aListing)    {      AddListingItems(aListing, SubFolders);
      AddListingItems(aListing, Files);
    }    protected void AddListingItems(FtpListing aListing, IEnumerable aFtpItems)
    {
      if (aFtpItems != null)
      {
        foreach (IFtpItem lFtpItem in aFtpItems)
        {
          FtpListingItem lListingItem = aListing.Add();
          lFtpItem.FillFtpListingItem(lListingItem);
        }
      }
    }
    #endregion

  }

}
