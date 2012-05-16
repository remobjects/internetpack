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
    public abstract class FtpFolder : FtpItem, IFtpFolder
    {
        protected FtpFolder(IFtpFolder parent, String name)
            : base(parent, name)
        {
            this.fSubFolderList = new Hashtable();
        }

        public override Int32 Size
        {
            get
            {
                return 0;
            }
            set
            {
                /* no-op */
            }
        }

        protected Hashtable SubFolderList
        {
            get
            {
                return fSubFolderList;
            }
        }
        private Hashtable fSubFolderList;

        #region IFtpFolder implementation
        public abstract IEnumerable SubFolders { get; }
        public abstract IEnumerable Files { get; }

        public abstract Boolean HasSubfolder(String folder);
        public abstract IFtpFolder GetSubFolder(String folder, VirtualFtpSession session);
        public abstract IFtpFolder CreateFolder(String folder, VirtualFtpSession session);
        public abstract void DeleteFolder(String folder, Boolean aRecursive, VirtualFtpSession session);

        public abstract Boolean HasFile(String filename);
        public abstract IFtpFile GetFile(String filename, VirtualFtpSession session);
        public abstract IFtpFile CreateFile(String filename, VirtualFtpSession session);
        public abstract void DeleteFile(String filename, VirtualFtpSession session);
        public abstract void RenameFileOrFolder(String oldFilename, String newFilename, VirtualFtpSession session);

        public abstract void RemoveItem(IFtpItem item);

        public IFtpFolder DigForSubFolder(String fullPath, VirtualFtpSession session)
        {
            if (fullPath.Length == 0)
                return this;

            Int32 lSeparatorIndex = fullPath.IndexOf('/');

            if (lSeparatorIndex < 0)
                return this.GetSubFolder(fullPath, session);

            String lSubFolder = fullPath.Substring(0, lSeparatorIndex);

            IFtpFolder lFolder = GetSubFolder(lSubFolder, session);

            if (lFolder == null)
                return null;

            fullPath = fullPath.Substring(lSeparatorIndex + 1);
            return lFolder.DigForSubFolder(fullPath, session);
        }

        public void FindBaseFolderForFilename(String path, out IFtpFolder folder, out String filename, VirtualFtpSession session)
        {
            if (path.IndexOf('/') != -1)
            {
                if (path.StartsWith("/"))
                {
                    folder = Root;
                    path = path.Substring(1); /* remove / */
                }
                else
                {
                    folder = this;
                }

                Int32 lSeparatorIndex = path.IndexOf('/');
                while (lSeparatorIndex >= 0)
                {
                    String lFolderName = path.Substring(0, lSeparatorIndex);
                    folder = folder.GetSubFolder(lFolderName, session);

                    if (folder == null || !folder.AllowBrowse(session))
                        throw new FtpException(550, String.Format("Folder \"{0}\" does not exists, or access denied.", path));

                    path = path.Substring(lSeparatorIndex + 1);
                    lSeparatorIndex = path.IndexOf('/');
                }
            }
            else
            {
                folder = this;
            }

            filename = path;
        }

        public String FullPath
        {
            get
            {
                if (this.Parent == null)
                    return "/";

                return this.Parent.FullPath + this.Name + "/";
            }
        }

        public virtual Boolean AllowBrowse(VirtualFtpSession session)
        {
            return AllowRead(session);
        }

        public virtual Boolean AllowGet(VirtualFtpSession session)
        {
            return AllowRead(session);
        }

        public virtual Boolean AllowPut(VirtualFtpSession session)
        {
            return AllowWrite(session);
        }

        public virtual Boolean AllowMkDir(VirtualFtpSession aSession)
        {
            return AllowWrite(aSession);
        }

        public virtual Boolean AllowDeleteItems(VirtualFtpSession session)
        {
            return AllowWrite(session);
        }

        public virtual Boolean AllowRenameItems(VirtualFtpSession session)
        {
            return AllowWrite(session);
        }

        public virtual Boolean AllowDeleteThis(VirtualFtpSession session)
        {
            return AllowWrite(session);
        }
        #endregion

        #region FolderListing
        public void ListFolderItems(FtpListing listing)
        {
            /* Add this folder (.) */
            FtpListingItem lListingItem;
            lListingItem = listing.Add();
            FillFtpListingItem(lListingItem, ".");

            /* Add parent folder (..) */
            if (Parent != null)
            {
                lListingItem = listing.Add();
                Parent.FillFtpListingItem(lListingItem, "..");
            }

            DoListFolderItems(listing);
        }

        public virtual void DoListFolderItems(FtpListing listing)
        {
            AddListingItems(listing, SubFolders);
            AddListingItems(listing, Files);
        }

        protected void AddListingItems(FtpListing listing, IEnumerable ftpItems)
        {
            if (ftpItems == null)
                return;

            foreach (IFtpItem item in ftpItems)
            {
                FtpListingItem lListingItem = listing.Add();
                item.FillFtpListingItem(lListingItem);
            }
        }
        #endregion
    }
}