/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

using System;

namespace RemObjects.InternetPack.Ftp.VirtualFtp
{
	public abstract class FtpItem : IFtpItem
	{
		protected FtpItem(IFtpFolder parent, String name)
		{
			Parent = parent;
			Name = name;

			UserRead = true;
			UserWrite = true;
			GroupRead = true;

			OwningUser = "system";
			OwningGroup = "system";

			Date = DateTime.Now;
		}

		#region General Item attributes
		public IFtpFolder Parent { get; set; }

		public IFtpFolder Root
		{
			get
			{
				IFtpFolder lParent = this is IFtpFolder ? (IFtpFolder) this : Parent;

				while (lParent.Parent != null)
				{
					lParent = lParent.Parent;
				}

				return lParent;
			}
		}

		public virtual String Name { get; set; }

		public virtual DateTime Date { get; set; }

		public abstract Int32 Size { get; set; }
		#endregion

		#region Rights
		public virtual String OwningUser
		{
			get
			{
				return fOwningUser;
			}
			set
			{
				fOwningUser = value;
			}
		}
		private String fOwningUser;

		public virtual String OwningGroup
		{
			get
			{
				return fOwningGroup;
			}
			set
			{
				fOwningGroup = value;
			}
		}
		private String fOwningGroup;

		public virtual Boolean AllowRead(VirtualFtpSession session)
		{
			if (Invalid)
				return false;

			if (session.IsFileAdmin)
				return true;

			if (session.Username == OwningUser && UserRead)
				return true;

			if (WorldRead)
				return true;

			return false;
		}

		public virtual Boolean AllowWrite(VirtualFtpSession session)
		{
			if (Invalid)
				return false;

			if (session.IsFileAdmin)
				return true;

			if (session.Username == OwningUser && UserWrite)
				return true;

			if (WorldWrite)
				return true;

			return false;
		}

		private Boolean[] fRights = new Boolean[6];

		public virtual Boolean UserRead
		{
			get
			{
				return fRights[0];
			}
			set
			{
				fRights[0] = value;
			}
		}

		public virtual Boolean UserWrite
		{
			get
			{
				return fRights[1];
			}
			set
			{
				fRights[1] = value;
			}
		}

		public virtual Boolean GroupRead
		{
			get
			{
				return fRights[2];
			}
			set
			{
				fRights[2] = value;
			}
		}

		public virtual Boolean GroupWrite
		{
			get
			{
				return fRights[3];
			}
			set
			{
				fRights[3] = value;
			}
		}

		public virtual Boolean WorldRead
		{
			get
			{
				return fRights[4];
			}
			set
			{
				fRights[4] = value;
			}
		}

		public virtual Boolean WorldWrite
		{
			get
			{
				return fRights[5];
			}
			set
			{
				fRights[5] = value;
			}
		}
		#endregion

		#region Invalidation
		public virtual Boolean Invalid
		{
			get
			{
				return fInvalid;
			}
			set
			{
				fInvalid = value;
			}
		}
		private Boolean fInvalid;

		public virtual void Invalidate()
		{
			this.Invalid = false;
		}
		#endregion

		public virtual void FillFtpListingItem(FtpListingItem item, String name)
		{
			FillFtpListingItem(item);
			item.FileName = name;
		}

		public virtual void FillFtpListingItem(FtpListingItem item)
		{
			item.Directory = (this is IFtpFolder);
			item.FileName = Name;
			item.FileDate = Date;
			item.Size = Size;
			item.User = OwningUser;
			item.Group = OwningGroup;
			item.UserRead = UserRead;
			item.UserWrite = UserWrite;
			item.UserExec = item.Directory && UserRead;
			item.GroupRead = GroupRead;
			item.GroupWrite = GroupWrite;
			item.GroupExec = item.Directory && GroupRead;
			item.OtherRead = WorldRead;
			item.OtherWrite = WorldWrite;
			item.OtherExec = item.Directory && WorldRead;
		}
	}
}
