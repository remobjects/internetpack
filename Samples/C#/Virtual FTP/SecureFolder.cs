using System;
using System.IO;

namespace RemObjects.InternetPack.Ftp.VirtualFtp
{

  /*
   * - Keep files open
   * - use different Key and IV per file
   * 
   * 
   * 
   */

  public class SecureFolder: VirtualFolder
  {
    public SecureFolder(IFtpFolder aParent, string aName, SecureStorage aStorage) : base(aParent, aName)
    {
      fStorage = aStorage;
      WorldRead = true;
      WorldWrite = true;
      OwningUser = "system";
      OwningGroup = "system";
    }

    public SecureFolder(IFtpFolder aParent, string aName, SecureStorage aStorage, bool aPrivcate) : this(aParent, aName, aStorage)
    {
      fStorage = aStorage;
      GroupRead = false;
      GroupWrite = false;
      WorldRead = false;
      WorldWrite = false;
      OwningUser = "system";
      OwningGroup = "system";
    }

    protected override IFtpFolder DoCreateFolder(string aFolderName, VirtualFtpSession aSession)    {      lock(this)
      {
        if (HasSubfolder(aFolderName))
          throw new FtpException(String.Format("Cannot create folder named \"{0}\", a folder with this name already exists.",aFolderName));        if (!AllowMkDir(aSession))
          throw new FtpException(550, String.Format("Cannot create folder named \"{0}\", permission denied.",aFolderName));        IFtpFolder lFolder = new SecureFolder(this, aFolderName, fStorage);        lFolder.OwningUser = aSession.Username;        Add(lFolder);        return lFolder;      }    }
    protected override IFtpFile DoCreateFile(string aFilename, VirtualFtpSession aSession)    {      if (HasFile(aFilename))
        throw new FtpException(String.Format("Cannot create file \"{0}\", a file with this name already exists.",aFilename));      if (!AllowMkDir(aSession))
        throw new FtpException(550, String.Format("Cannot create file \"{0}\", permission denied.",aFilename));      SecureFile lFile = new SecureFile(this, aFilename, fStorage);      lFile.OwningUser = aSession.Username;      Add(lFile);      return lFile;    }
    private SecureStorage fStorage;  }

}
