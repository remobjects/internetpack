using System;
using System.IO;
using System.Collections;

namespace RemObjects.InternetPack.Ftp.VirtualFtp
{

  public class VirtualFolder : FtpFolder
	{
		public VirtualFolder(IFtpFolder aParent, string aName) : this(aParent, aName, "system", "system", false)
		{
      fFileList = new Hashtable();
      WorldRead = true;
      WorldWrite = false;
      Date = DateTime.Now;
    }

    public VirtualFolder(IFtpFolder aParent, string aName, string aOwningUser, string aOwningGroup, bool aWorldWritable) : base(aParent, aName)
    {
      fFileList = new Hashtable();
      WorldRead = true;
      WorldWrite = aWorldWritable;
      OwningUser = aOwningUser;
      OwningGroup = aOwningGroup;
      Date = DateTime.Now;
    }

    #region Elements
    private Hashtable fFileList;    protected Hashtable FileList    {      get { return fFileList; }    }

    public override IEnumerable SubFolders     {       get       {         return SubFolderList.Values;       }     }    public override IEnumerable Files { get { return FileList.Values; } }    public void Add(IFtpItem aItem)
    {
      lock (this)
      {
        if (aItem is IFtpFolder)
        {
          aItem.Parent = this;
          SubFolderList.Add(aItem.Name.ToLower(), aItem);
        }
        else if (aItem is IFtpFile)
        {
          aItem.Parent = this;
          FileList.Add(aItem.Name.ToLower(), aItem);
        }
      }
    }
    #endregion    #region Has*    public override bool HasSubfolder(string aFolderName)    {      return SubFolderList.ContainsKey(aFolderName.ToLower());    }    public override bool HasFile(string aFileame)    {      return FileList.ContainsKey(aFileame.ToLower());    }    #endregion                   #region Get*    public override IFtpFolder GetSubFolder(string aFolderName, VirtualFtpSession aSession)    {      if (!AllowBrowse(aSession))
        throw new FtpException(550, String.Format("Cannot access folder \"{0}\", permission to access items in this folder denied.",aFolderName));            IFtpFolder lFolder = SubFolderList[aFolderName.ToLower()] as IFtpFolder;      if (lFolder != null && !lFolder.AllowBrowse(aSession))
        throw new FtpException(550, String.Format("Cannot access folder \"{0}\", permission to browse folder denied.",aFolderName));      return lFolder;    }    public override IFtpFile GetFile(string aFilename, VirtualFtpSession aSession)    {      if (!HasFile(aFilename))
        throw new FtpException(String.Format("A file named \"{0}\" does not exists.",aFilename));      if (!AllowBrowse(aSession))
        throw new FtpException(550,String.Format("Cannot access file \"{0}\", permission to access files in this folder denied.",aFilename));            IFtpFile lFile = FileList[aFilename.ToLower()] as IFtpFile;      if (!lFile.AllowRead(aSession))
        throw new FtpException(550, String.Format("Cannot access file \"{0}\", permission to access file denied.",aFilename));      return lFile;    }    #endregion
    #region Create*
    public override sealed IFtpFolder CreateFolder(string aFolderName, VirtualFtpSession aSession)    {      if (!AllowMkDir(aSession))
        throw new FtpException(550, String.Format("Cannot create folder \"{0}\", permission to mkdir in this folder denied.",aFolderName));      return DoCreateFolder(aFolderName, aSession);    }    public override sealed IFtpFile CreateFile(string aFilename, VirtualFtpSession aSession)    {      if (!AllowPut(aSession))
        throw new FtpException(550, String.Format("Cannot create file \"{0}\", permission to upload to this folder denied.",aFilename));            return DoCreateFile(aFilename, aSession);    }
    #endregion

    #region Create* virtuals
    protected virtual IFtpFolder DoCreateFolder(string aFolderName, VirtualFtpSession aSession)    {      throw new FtpException(550, String.Format("You cannot create subfolders in a {0}.",this.GetType().Name));    }    protected virtual IFtpFile DoCreateFile(string aFilename, VirtualFtpSession aSession)    {      throw new FtpException(550, String.Format("You cannot create files in a {0}.",this.GetType().Name));    }    #endregion
    #region Delete*
    public override void DeleteFolder(string aFolderName, bool aRecursive, VirtualFtpSession aSession)    {      if (!HasSubfolder(aFolderName))
        throw new FtpException(String.Format("A folder named \"{0}\" does not exists.",aFolderName));
      if (!AllowDeleteItems(aSession))
        throw new FtpException(550, String.Format("Cannot delete folder \"{0}\", permission to delete from this folder denied.",aFolderName));
      IFtpFolder lFolder = GetSubFolder(aFolderName, aSession);
      if (!lFolder.AllowDeleteThis(aSession))
        throw new FtpException(550, String.Format("Cannot delete folder \"{0}\", permission to delete folder denied.",aFolderName));
      lock(this)
      {
        lFolder.Invalidate();
        SubFolderList.Remove(aFolderName.ToLower());      }    }    public override void DeleteFile(string aFilename, VirtualFtpSession aSession)    {      if (!HasFile(aFilename))
        throw new FtpException(String.Format("A file named \"{0}\" does not exists.",aFilename));
      if (!AllowDeleteItems(aSession))
        throw new FtpException(550, String.Format("Cannot delete fike \"{0}\", permission to delete from this folder denied.",aFilename));
      IFtpFile lFile = GetFile(aFilename, aSession);
      if (!lFile.AllowDelete(aSession))
        throw new FtpException(550, String.Format("Cannot delete file \"{0}\", permission to delete file denied.",aFilename));
      lock(this)
      {
        lFile.Invalidate();
        FileList.Remove(aFilename.ToLower());      }    }    public override void RemoveItem(IFtpItem aItem)    {      if (FileList.ContainsValue(aItem))      {        FileList.Remove(aItem.Name);      }    }
    #endregion    public override void RenameFileOrFolder(string aOldFilename, string aNewFilename, VirtualFtpSession aSession)    {      if (HasSubfolder(aOldFilename))
      {
        if (!AllowRenameItems(aSession))
          throw new FtpException(550, String.Format("Cannot rename folder \"{0}\", permission to rename in this folder denied.",aOldFilename));      }
      else if (HasFile(aOldFilename))
      {
        if (!AllowRenameItems(aSession))
          throw new FtpException(550, String.Format("Cannot rename file \"{0}\", permission to rename in this folder denied.",aOldFilename));
        IFtpFile lFile = GetFile(aOldFilename, aSession);
        if (!lFile.AllowRename(aSession))
          throw new FtpException(550, String.Format("Cannot rename file \"{0}\", permission to rename file denied.",aOldFilename));
        lock (this)
        {
          lFile.Name = aNewFilename;
        }
      }
      else
      {
        throw new FtpException(String.Format("A file or folder named \"{0}\" does not exists.",aOldFilename));      }    }  }
    
}
