/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

using System;
using System.Collections;
using System.Net;

namespace RemObjects.InternetPack.Ftp.VirtualFtp
{
	public class User
	{
		public User(String username, String password)
		{
			this.Username = username;
			this.Password = password;
		}

		public String Username { get; set; }

		public String Password { get; set; }
	}

	public class UserManager : Hashtable, IFtpUserManager
	{
		public UserManager()
		{
		}

		public virtual Boolean CheckIP(EndPoint remote, EndPoint local)
		{
			return true;
		}

		public virtual Boolean CheckLogin(String username, String password, VirtualFtpSession session)
		{
			User lUser = (User)this[username.ToLower()];

			return lUser != null && lUser.Password == password;
		}

		public void AddUser(String username, String password)
		{
			this.Add(username.ToLower(), new User(username, password));
		}

		public void AddUser(User user)
		{
			this.Add(user.Username.ToLower(), user);
		}
	}
}
