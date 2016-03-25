/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

using System;
using RemObjects.InternetPack.CommandBased;

namespace RemObjects.InternetPack.Ftp.VirtualFtp
{
#if FULLFRAMEWORK
  [ System.Drawing.ToolboxBitmap(typeof(RemObjects.InternetPack.Server), "Glyphs.VirtualFtpServer.bmp") ]
#endif
	public class VirtualFtpServer : FtpServer
	{
		public VirtualFtpServer()
		{
		}

		#region Properties
		public IFtpFolder RootFolder { get; set; }

		public IFtpUserManager UserManager { get; set; }
		#endregion

		protected override Type GetDefaultSessionClass()
		{
			return typeof(VirtualFtpSession);
		}

		#region Login & IP Check
		protected override void InvokeOnUserLogin(FtpUserLoginEventArgs e)
		{
			if (UserManager == null)
				return;

			VirtualFtpSession lSession = (VirtualFtpSession)e.Session;
			e.LoginOk = UserManager.CheckLogin(e.UserName, e.Password, lSession);
			base.InvokeOnUserLogin(e);
		}

		protected override void InvokeOnClientConnected(SessionEventArgs e)
		{
			((VirtualFtpSession)e.Session).CurrentFolder = RootFolder;

			if (UserManager == null)
				return;

			if (!UserManager.CheckIP(e.Connection.RemoteEndPoint, e.Connection.LocalEndPoint))
			{
				e.Connection.Disconnect();
				throw new Exception("Access not allowed from this IP.");
			}

			base.InvokeOnClientConnected(e);
		}
		#endregion

		#region Folder handling
		protected override void InvokeOnGetListing(FtpGetListingArgs e)
		{
			VirtualFtpSession lSession = (VirtualFtpSession)e.Session;

			lSession.CurrentFolder.ListFolderItems(e.Listing);

			base.InvokeOnGetListing(e);
		}

		protected override void InvokeOnChangeDirectory(FtpChangeDirectoryArgs e)
		{
			String lPath = e.NewDirectory;

			if (lPath.IndexOf('/') != 0)
				throw new Exception(String.Format("Not an absolute path: \"{0}\"", lPath));

			VirtualFtpSession lSession = (VirtualFtpSession)e.Session;
			IFtpFolder lFolder = RootFolder.DigForSubFolder(lPath.Substring(1), lSession);

			if (lFolder != null)
				((VirtualFtpSession)e.Session).CurrentFolder = lFolder;

			e.ChangeDirOk = (lFolder != null);
			base.InvokeOnChangeDirectory(e);
		}

		protected override void InvokeOnMakeDirectory(FtpFileEventArgs e)
		{
			VirtualFtpSession lSession = (VirtualFtpSession)e.Session;

			IFtpFolder lFolder;
			String lFilename;
			lSession.CurrentFolder.FindBaseFolderForFilename(e.FileName, out lFolder, out lFilename, lSession);

			lFolder.CreateFolder(lFilename, lSession);
			e.Ok = true;

		}

		protected override void InvokeOnDeleteDirectory(FtpFileEventArgs e)
		{
			VirtualFtpSession lSession = (VirtualFtpSession)e.Session;

			IFtpFolder lFolder;
			String lFilename;
			lSession.CurrentFolder.FindBaseFolderForFilename(e.FileName, out lFolder, out lFilename, lSession);

			lFolder.DeleteFolder(lFilename, false, lSession);
			e.Ok = true;
		}
		#endregion

		#region File Handling
		protected override void InvokeOnCanStoreFile(FtpTransferEventArgs e)
		{
			VirtualFtpSession lSession = (VirtualFtpSession)e.Session;

			IFtpFolder lFolder;
			String lFilename;
			lSession.CurrentFolder.FindBaseFolderForFilename(e.FileName, out lFolder, out lFilename, lSession);

			e.Ok = lFolder.AllowPut(lSession);
		}

		protected override void InvokeOnCanRetrieveFile(FtpTransferEventArgs e)
		{
			VirtualFtpSession lSession = (VirtualFtpSession)e.Session;

			IFtpFolder lFolder;
			String lFilename;
			lSession.CurrentFolder.FindBaseFolderForFilename(e.FileName, out lFolder, out lFilename, lSession);

			IFtpFile lFile = lFolder.GetFile(lFilename, lSession);
			e.Ok = (lFile != null && lFile.AllowRead(lSession));
		}

		protected override void InvokeOnStoreFile(FtpTransferEventArgs e)
		{
			VirtualFtpSession lSession = (VirtualFtpSession)e.Session;

			IFtpFolder lFolder;
			String lFilename;
			lSession.CurrentFolder.FindBaseFolderForFilename(e.FileName, out lFolder, out lFilename, lSession);

			IFtpFile lFile = lFolder.CreateFile(lFilename, lSession);
			lFile.CreateFile(e.DataChannel);
			e.Ok = true;
		}

		protected override void InvokeOnRetrieveFile(FtpTransferEventArgs e)
		{
			VirtualFtpSession lSession = (VirtualFtpSession)e.Session;

			IFtpFolder lFolder;
			String lFilename;
			lSession.CurrentFolder.FindBaseFolderForFilename(e.FileName, out lFolder, out lFilename, lSession);

			IFtpFile lFile = lFolder.GetFile(lFilename, lSession);
			lFile.GetFile(e.DataChannel);
			e.Ok = true;
		}

		protected override void InvokeOnRename(FtpRenameEventArgs e)
		{
			VirtualFtpSession lSession = (VirtualFtpSession)e.Session;

			IFtpFolder lFolder;
			String lFilename;
			lSession.CurrentFolder.FindBaseFolderForFilename(e.FileName, out lFolder, out lFilename, lSession);

			lFolder.RenameFileOrFolder(lFilename, e.NewFileName, lSession);
			e.Ok = true;
		}

		protected override void InvokeOnDelete(FtpFileEventArgs e)
		{
			VirtualFtpSession lSession = (VirtualFtpSession)e.Session;

			IFtpFolder lFolder;
			String lFilename;
			lSession.CurrentFolder.FindBaseFolderForFilename(e.FileName, out lFolder, out lFilename, lSession);

			lFolder.DeleteFile(lFilename, lSession);
			e.Ok = true;
		}
		#endregion
	}
}