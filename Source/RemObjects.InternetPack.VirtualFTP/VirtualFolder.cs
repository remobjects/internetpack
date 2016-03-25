/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

using System;
using System.Collections;

namespace RemObjects.InternetPack.Ftp.VirtualFtp
{

	public class VirtualFolder : FtpFolder
	{
		public VirtualFolder(IFtpFolder parent, String name)
			: this(parent, name, "system", "system", false)
		{
		}

		public VirtualFolder(IFtpFolder parent, String name, String ownerUser, String ownerGroup, Boolean aWorldWritable)
			: base(parent, name)
		{
			this.fFileList = new Hashtable();
			this.WorldRead = true;
			this.WorldWrite = aWorldWritable;
			this.OwningUser = ownerUser;
			this.OwningGroup = ownerGroup;
			this.Date = DateTime.Now;
		}

		#region Elements
		protected Hashtable FileList
		{
			get
			{
				return fFileList;
			}
		}
		private readonly Hashtable fFileList;

		public override IEnumerable SubFolders
		{
			get
			{
				return SubFolderList.Values;
			}
		}

		public override IEnumerable Files
		{
			get
			{
				return FileList.Values;
			}
		}

		public void Add(IFtpItem item)
		{
			lock (this)
			{
				if (item is IFtpFolder)
				{
					item.Parent = this;
					SubFolderList.Add(item.Name.ToLower(), item);
				}
				else if (item is IFtpFile)
				{
					item.Parent = this;
					FileList.Add(item.Name.ToLower(), item);
				}
			}
		}
		#endregion

		#region Has*
		public override Boolean HasSubfolder(String folder)
		{
			return SubFolderList.ContainsKey(folder.ToLower());
		}

		public override Boolean HasFile(String fileName)
		{
			return FileList.ContainsKey(fileName.ToLower());
		}
		#endregion

		#region Get*
		public override IFtpFolder GetSubFolder(String folder, VirtualFtpSession session)
		{
			if (!AllowBrowse(session))
				throw new FtpException(550, String.Format("Cannot access folder \"{0}\", permission to access items in this folder denied.", folder));

			IFtpFolder lFolder = SubFolderList[folder.ToLower()] as IFtpFolder;
			if (lFolder != null && !lFolder.AllowBrowse(session))
				throw new FtpException(550, String.Format("Cannot access folder \"{0}\", permission to browse folder denied.", folder));

			return lFolder;
		}

		public override IFtpFile GetFile(String fileName, VirtualFtpSession session)
		{
			if (!HasFile(fileName))
				throw new FtpException(String.Format("A file named \"{0}\" does not exists.", fileName));

			if (!AllowBrowse(session))
				throw new FtpException(550, String.Format("Cannot access file \"{0}\", permission to access files in this folder denied.", fileName));

			IFtpFile lFile = FileList[fileName.ToLower()] as IFtpFile;
			if ((lFile == null) || !lFile.AllowRead(session))
			{
				throw new FtpException(550, String.Format("Cannot access file \"{0}\", permission to access file denied.", fileName));
			}

			return lFile;
		}
		#endregion

		#region Create*
		public sealed override IFtpFolder CreateFolder(String folder, VirtualFtpSession session)
		{
			if (!AllowMkDir(session))
				throw new FtpException(550, String.Format("Cannot create folder \"{0}\", permission to mkdir in this folder denied.", folder));
			return DoCreateFolder(folder, session);
		}

		public sealed override IFtpFile CreateFile(String fileName, VirtualFtpSession session)
		{
			if (!AllowPut(session))
				throw new FtpException(550, String.Format("Cannot create file \"{0}\", permission to upload to this folder denied.", fileName));

			return DoCreateFile(fileName, session);
		}
		#endregion

		#region Create* virtuals
		protected virtual IFtpFolder DoCreateFolder(String folder, VirtualFtpSession session)
		{
			throw new FtpException(550, String.Format("You cannot create subfolders in a {0}.", this.GetType().Name));
		}

		protected virtual IFtpFile DoCreateFile(String fileName, VirtualFtpSession session)
		{
			throw new FtpException(550, String.Format("You cannot create files in a {0}.", this.GetType().Name));
		}
		#endregion

		#region Delete*
		public override void DeleteFolder(String folder, Boolean recursive, VirtualFtpSession session)
		{
			if (!HasSubfolder(folder))
				throw new FtpException(String.Format("A folder named \"{0}\" does not exists.", folder));

			if (!AllowDeleteItems(session))
				throw new FtpException(550, String.Format("Cannot delete folder \"{0}\", permission to delete from this folder denied.", folder));

			IFtpFolder lFolder = GetSubFolder(folder, session);
			if (!lFolder.AllowDeleteThis(session))
				throw new FtpException(550, String.Format("Cannot delete folder \"{0}\", permission to delete folder denied.", folder));

			lock (this)
			{
				lFolder.Invalidate();
				SubFolderList.Remove(folder.ToLower());
			}
		}

		public override void DeleteFile(String fileName, VirtualFtpSession session)
		{
			if (!HasFile(fileName))
				throw new FtpException(String.Format("A file named \"{0}\" does not exists.", fileName));

			if (!AllowDeleteItems(session))
				throw new FtpException(550, String.Format("Cannot delete fike \"{0}\", permission to delete from this folder denied.", fileName));

			IFtpFile lFile = GetFile(fileName, session);
			if (!lFile.AllowDelete(session))
				throw new FtpException(550, String.Format("Cannot delete file \"{0}\", permission to delete file denied.", fileName));

			lock (this)
			{
				lFile.Invalidate();
				FileList.Remove(fileName.ToLower());
			}
		}

		public override void RemoveItem(IFtpItem item)
		{
			if (FileList.ContainsValue(item))
			{
				FileList.Remove(item.Name);
			}
		}
		#endregion

		public override void RenameFileOrFolder(String oldFileName, String newFileName, VirtualFtpSession session)
		{
			if (HasSubfolder(oldFileName))
			{
				if (!AllowRenameItems(session))
					throw new FtpException(550, String.Format("Cannot rename folder \"{0}\", permission to rename in this folder denied.", oldFileName));
			}
			else if (HasFile(oldFileName))
			{
				if (!AllowRenameItems(session))
					throw new FtpException(550, String.Format("Cannot rename file \"{0}\", permission to rename in this folder denied.", oldFileName));

				IFtpFile lFile = GetFile(oldFileName, session);
				if (!lFile.AllowRename(session))
					throw new FtpException(550, String.Format("Cannot rename file \"{0}\", permission to rename file denied.", oldFileName));

				lock (this)
				{
					lFile.Name = newFileName;
				}
			}
			else
			{
				throw new FtpException(String.Format("A file or folder named \"{0}\" does not exists.", oldFileName));
			}
		}
	}
}
