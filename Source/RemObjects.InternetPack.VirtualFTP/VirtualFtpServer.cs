using System;
using System.IO;
using System.Net;
using System.Collections;
using RemObjects.InternetPack;
using RemObjects.InternetPack.CommandBased;
using RemObjects.InternetPack.Ftp;

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
		private IFtpFolder fRootFolder;
		public IFtpFolder RootFolder
		{
			get { return fRootFolder; }
			set { fRootFolder = value; }
		}

		private IFtpUserManager fUserManager;
		public IFtpUserManager UserManager
		{
			get { return fUserManager; }
			set { fUserManager = value; }
		}
		#endregion

		protected override Type GetDefaultSessionClass()
		{
			return typeof(VirtualFtpSession);
		}

		#region Login & IP Check
		protected override void InvokeOnUserLogin(FtpUserLoginEventArgs ea)
		{
#if DEBUGSERVER
			Debug.EnterMethod("Login({0})", ea.UserName);
			try
			{
#endif
				if (fUserManager == null) return;
				VirtualFtpSession lSession = (VirtualFtpSession)ea.Session;
				ea.LoginOk = fUserManager.CheckLogin(ea.UserName, ea.Password, lSession);
#if DEBUGSERVER
				Debug.Write("Login result " + ea.LoginOk);
#endif
				base.InvokeOnUserLogin(ea);
#if DEBUGSERVER
			}
			finally
			{
				Debug.ExitMethod("Login({0}):{1}", ea.UserName, ea.LoginOk);
			}
#endif
		}

		protected override void InvokeOnClientConnected(SessionEventArgs ea)
		{
			((VirtualFtpSession)ea.Session).CurrentFolder = fRootFolder;
			if (fUserManager == null) return;
			if (!fUserManager.CheckIP(ea.Connection.RemoteEndPoint, ea.Connection.LocalEndPoint))
			{
				ea.Connection.Disconnect();
				throw new Exception("Access not allowed from this IP.");
			}
			base.InvokeOnClientConnected(ea);
		}
		#endregion

		#region Folder handling
		protected override void InvokeOnGetListing(FtpGetListingArgs ea)
		{
			VirtualFtpSession lSession = (VirtualFtpSession)ea.Session;
#if DEBUGSERVER
			Debug.Write("Getting Listing for {0}", lSession.CurrentFolder.FullPath);
#endif
			lSession.CurrentFolder.ListFolderItems(ea.Listing);

			base.InvokeOnGetListing(ea);
		}

		protected override void InvokeOnChangeDirectory(FtpChangeDirectoryArgs ea)
		{
			string lPath = ea.NewDirectory;

			if (lPath.IndexOf('/') != 0)
				throw new Exception(String.Format("Not an absolute path: \"{0}\"", lPath));

			VirtualFtpSession lSession = (VirtualFtpSession)ea.Session;
			IFtpFolder lFolder = fRootFolder.DigForSubFolder(lPath.Substring(1), lSession);

			if (lFolder != null)
			{
				((VirtualFtpSession)ea.Session).CurrentFolder = lFolder;
			}
			ea.ChangeDirOk = (lFolder != null);
			base.InvokeOnChangeDirectory(ea);
		}

		protected override void InvokeOnMakeDirectory(FtpFileEventArgs ea)
		{
			VirtualFtpSession lSession = (VirtualFtpSession)ea.Session;

			IFtpFolder lFolder;
			string lFilename;

			lSession.CurrentFolder.FindBaseFolderForFilename(ea.FileName, out lFolder, out lFilename, lSession);
			lFolder.CreateFolder(lFilename, lSession);
			ea.Ok = true;

		}

		protected override void InvokeOnDeleteDirectory(FtpFileEventArgs ea)
		{
			VirtualFtpSession lSession = (VirtualFtpSession)ea.Session;
			IFtpFolder lFolder;
			string lFilename;

			lSession.CurrentFolder.FindBaseFolderForFilename(ea.FileName, out lFolder, out lFilename, lSession);
			lFolder.DeleteFolder(lFilename, false, lSession);
			ea.Ok = true;
		}
		#endregion

		#region File Handling
		protected override void InvokeOnCanStoreFile(FtpTransferEventArgs ea)
		{
			VirtualFtpSession lSession = (VirtualFtpSession)ea.Session;
			IFtpFolder lFolder;
			string lFilename;

			lSession.CurrentFolder.FindBaseFolderForFilename(ea.FileName, out lFolder, out lFilename, lSession);
			ea.Ok = lFolder.AllowPut(lSession);
		}

		protected override void InvokeOnCanRetrieveFile(FtpTransferEventArgs ea)
		{
			VirtualFtpSession lSession = (VirtualFtpSession)ea.Session;
			IFtpFolder lFolder;
			string lFilename;

			lSession.CurrentFolder.FindBaseFolderForFilename(ea.FileName, out lFolder, out lFilename, lSession);
			IFtpFile lFile = lFolder.GetFile(lFilename, lSession);
			ea.Ok = (lFile != null && lFile.AllowRead(lSession));
		}

		protected override void InvokeOnStoreFile(FtpTransferEventArgs ea)
		{
			VirtualFtpSession lSession = (VirtualFtpSession)ea.Session;
			IFtpFolder lFolder;
			string lFilename;

			lSession.CurrentFolder.FindBaseFolderForFilename(ea.FileName, out lFolder, out lFilename, lSession);
			IFtpFile lFile = lFolder.CreateFile(lFilename, lSession);
			lFile.CreateFile(ea.DataChannel);
			ea.Ok = true;
		}

		protected override void InvokeOnRetrieveFile(FtpTransferEventArgs ea)
		{
			VirtualFtpSession lSession = (VirtualFtpSession)ea.Session;
			IFtpFolder lFolder;
			string lFilename;

			lSession.CurrentFolder.FindBaseFolderForFilename(ea.FileName, out lFolder, out lFilename, lSession);
			IFtpFile lFile = lFolder.GetFile(lFilename, lSession);
			lFile.GetFile(ea.DataChannel);
			ea.Ok = true;
		}

		protected override void InvokeOnRename(FtpRenameEventArgs ea)
		{
			VirtualFtpSession lSession = (VirtualFtpSession)ea.Session;
			IFtpFolder lFolder;
			string lFilename;

			lSession.CurrentFolder.FindBaseFolderForFilename(ea.FileName, out lFolder, out lFilename, lSession);
			lFolder.RenameFileOrFolder(lFilename, ea.NewFileName, lSession);
			ea.Ok = true;
		}

		protected override void InvokeOnDelete(FtpFileEventArgs ea)
		{
			VirtualFtpSession lSession = (VirtualFtpSession)ea.Session;
			IFtpFolder lFolder;
			string lFilename;

			lSession.CurrentFolder.FindBaseFolderForFilename(ea.FileName, out lFolder, out lFilename, lSession);
			lFolder.DeleteFile(lFilename, lSession);
			ea.Ok = true;
		}
		#endregion

	}

}
