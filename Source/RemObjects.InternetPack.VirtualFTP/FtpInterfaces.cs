/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

using System;
using System.Collections;
using System.IO;
using System.Net;

namespace RemObjects.InternetPack.Ftp.VirtualFtp
{
	public interface IFtpItem
	{
		IFtpFolder Parent { get; set; }
		String Name { get; set; }
		DateTime Date { get; }

		String OwningUser { get; set; }
		String OwningGroup { get; set; }

		Boolean AllowRead(VirtualFtpSession session);
		Boolean AllowWrite(VirtualFtpSession session);

		void FillFtpListingItem(FtpListingItem item);
		void FillFtpListingItem(FtpListingItem item, String aAsName);

		void Invalidate();
	}

	public interface IFtpFolder : IFtpItem
	{
		IFtpFolder Root { get; }

		IEnumerable SubFolders { get; }
		IEnumerable Files { get; }

		Boolean HasSubfolder(String folder);
		IFtpFolder GetSubFolder(String folder, VirtualFtpSession session);
		IFtpFolder CreateFolder(String folder, VirtualFtpSession session);
		void DeleteFolder(String folder, Boolean recursive, VirtualFtpSession session);
		void ListFolderItems(FtpListing listing);

		IFtpFolder DigForSubFolder(String fullPath, VirtualFtpSession session);
		void FindBaseFolderForFilename(String path, out IFtpFolder folder, out String filename, VirtualFtpSession session);

		String FullPath { get; }

		Boolean HasFile(String fileName);
		IFtpFile GetFile(String fileName, VirtualFtpSession session);
		IFtpFile CreateFile(String fileName, VirtualFtpSession session);

		void DeleteFile(String fileName, VirtualFtpSession session);
		void RenameFileOrFolder(String oldFileName, String newFileName, VirtualFtpSession session);

		void RemoveItem(IFtpItem item);

		Boolean AllowBrowse(VirtualFtpSession session);
		Boolean AllowGet(VirtualFtpSession session);
		Boolean AllowPut(VirtualFtpSession session);
		Boolean AllowMkDir(VirtualFtpSession session);
		Boolean AllowDeleteItems(VirtualFtpSession session);
		Boolean AllowRenameItems(VirtualFtpSession session);
		Boolean AllowDeleteThis(VirtualFtpSession session);
	}

	public interface IFtpFile : IFtpItem
	{
		Int32 Size { get; }

		void GetFile(Stream destination);
		void CreateFile(Stream source);

		Boolean AllowGet(VirtualFtpSession session);
		Boolean AllowAppend(VirtualFtpSession session);
		Boolean AllowDelete(VirtualFtpSession session);
		Boolean AllowRename(VirtualFtpSession session);
	}

	public interface IFtpUserManager
	{
		Boolean CheckIP(EndPoint remote, EndPoint local);
		Boolean CheckLogin(String username, String password, VirtualFtpSession session);
	}
}
