using System;
using System.Collections;
using System.IO;
using System.Net;
using RemObjects.InternetPack;

namespace RemObjects.InternetPack.Ftp.VirtualFtp
{

  public interface IFtpItem
  {
    IFtpFolder Parent { get; set; }    string Name { get; set; }    DateTime Date { get; }    string OwningUser { get; set;  }    string OwningGroup { get; set; }    bool AllowRead(VirtualFtpSession aSession);    bool AllowWrite(VirtualFtpSession aSession);    void FillFtpListingItem(FtpListingItem aItem);    void FillFtpListingItem(FtpListingItem aItem, string aAsName);    void Invalidate();  }

  public interface IFtpFolder: IFtpItem
  {
    IFtpFolder Root { get ; }    IEnumerable SubFolders { get ; }    IEnumerable Files { get ; }    bool HasSubfolder(string aFolderName);    IFtpFolder GetSubFolder(string aFolderName, VirtualFtpSession aSession);    IFtpFolder CreateFolder(string aFolderName, VirtualFtpSession aSession);    void DeleteFolder(string aFolderName, bool aRecursive, VirtualFtpSession aSession);    void ListFolderItems(FtpListing aListing);    IFtpFolder DigForSubFolder(string aFullPath, VirtualFtpSession aSession);    void FindBaseFolderForFilename(string aPath, out IFtpFolder aFolder, out string aFilename, VirtualFtpSession aSession);    string FullPath { get ; }    bool HasFile(string aFilename);    IFtpFile GetFile(string aFilename, VirtualFtpSession aSession);    IFtpFile CreateFile(string aFilename, VirtualFtpSession aSession);    void DeleteFile(string aFilename, VirtualFtpSession aSession);    void RenameFileOrFolder(string aOldFilename, string aNewFilename, VirtualFtpSession aSession);    void RemoveItem(IFtpItem aItem);    bool AllowBrowse(VirtualFtpSession aSession);    bool AllowGet(VirtualFtpSession aSession);    bool AllowPut(VirtualFtpSession aSession);    bool AllowMkDir(VirtualFtpSession aSession);    bool AllowDeleteItems(VirtualFtpSession aSession);    bool AllowRenameItems(VirtualFtpSession aSession);    bool AllowDeleteThis(VirtualFtpSession aSession);  }

  public interface IFtpFile: IFtpItem
  {
    int Size { get ; }    /* ToDo: add file rights */    void GetFile(Stream aToStream);    void CreateFile(Stream aFromStream);
    bool AllowGet(VirtualFtpSession aSession);    bool AllowAppend(VirtualFtpSession aSession);    bool AllowDelete(VirtualFtpSession aSession);    bool AllowRename(VirtualFtpSession aSession);  }

  public interface IFtpUserManager
  {
    bool CheckIP(EndPoint aRemote, EndPoint aLocal);
    bool CheckLogin(string aUsername, string aPassword, VirtualFtpSession aSession);  }


}
