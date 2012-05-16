/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET - Virtual FTP Library
  (c)opyright RemObjects Software, LLC. 2003-2012. All rights reserved.

  Using this code requires a valid license of the RemObjects Internet Pack
  which can be obtained at http://www.remobjects.com?ip.
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
        private Hashtable fFileList;

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

        public override Boolean HasFile(String filename)
        {
            return FileList.ContainsKey(filename.ToLower());
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

        public override IFtpFile GetFile(String filename, VirtualFtpSession session)
        {
            if (!HasFile(filename))
                throw new FtpException(String.Format("A file named \"{0}\" does not exists.", filename));

            if (!AllowBrowse(session))
                throw new FtpException(550, String.Format("Cannot access file \"{0}\", permission to access files in this folder denied.", filename));

            IFtpFile lFile = FileList[filename.ToLower()] as IFtpFile;
            if (!lFile.AllowRead(session))
                throw new FtpException(550, String.Format("Cannot access file \"{0}\", permission to access file denied.", filename));

            return lFile;
        }
        #endregion

        #region Create*
        public override sealed IFtpFolder CreateFolder(String folder, VirtualFtpSession session)
        {
            if (!AllowMkDir(session))
                throw new FtpException(550, String.Format("Cannot create folder \"{0}\", permission to mkdir in this folder denied.", folder));
            return DoCreateFolder(folder, session);
        }

        public override sealed IFtpFile CreateFile(String filename, VirtualFtpSession session)
        {
            if (!AllowPut(session))
                throw new FtpException(550, String.Format("Cannot create file \"{0}\", permission to upload to this folder denied.", filename));

            return DoCreateFile(filename, session);
        }
        #endregion

        #region Create* virtuals
        protected virtual IFtpFolder DoCreateFolder(String folder, VirtualFtpSession session)
        {
            throw new FtpException(550, String.Format("You cannot create subfolders in a {0}.", this.GetType().Name));
        }

        protected virtual IFtpFile DoCreateFile(String filename, VirtualFtpSession session)
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

        public override void DeleteFile(String filename, VirtualFtpSession session)
        {
            if (!HasFile(filename))
                throw new FtpException(String.Format("A file named \"{0}\" does not exists.", filename));

            if (!AllowDeleteItems(session))
                throw new FtpException(550, String.Format("Cannot delete fike \"{0}\", permission to delete from this folder denied.", filename));

            IFtpFile lFile = GetFile(filename, session);
            if (!lFile.AllowDelete(session))
                throw new FtpException(550, String.Format("Cannot delete file \"{0}\", permission to delete file denied.", filename));

            lock (this)
            {
                lFile.Invalidate();
                FileList.Remove(filename.ToLower());
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

        public override void RenameFileOrFolder(String oldFilename, String newFilename, VirtualFtpSession session)
        {
            if (HasSubfolder(oldFilename))
            {
                if (!AllowRenameItems(session))
                    throw new FtpException(550, String.Format("Cannot rename folder \"{0}\", permission to rename in this folder denied.", oldFilename));
            }
            else if (HasFile(oldFilename))
            {
                if (!AllowRenameItems(session))
                    throw new FtpException(550, String.Format("Cannot rename file \"{0}\", permission to rename in this folder denied.", oldFilename));

                IFtpFile lFile = GetFile(oldFilename, session);
                if (!lFile.AllowRename(session))
                    throw new FtpException(550, String.Format("Cannot rename file \"{0}\", permission to rename file denied.", oldFilename));

                lock (this)
                {
                    lFile.Name = newFilename;
                }
            }
            else
            {
                throw new FtpException(String.Format("A file or folder named \"{0}\" does not exists.", oldFilename));
            }
        }
    }
}
