/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET - Virtual FTP Library
  (c)opyright RemObjects Software, LLC. 2003-2012. All rights reserved.

  Using this code requires a valid license of the RemObjects Internet Pack
  which can be obtained at http://www.remobjects.com?ip.
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

        public String Username
        {
            get
            {
                return fUsername;
            }
            set
            {
                fUsername = value;
            }
        }
        private String fUsername;

        public String Password
        {
            get
            {
                return fPassword;
            }
            set
            {
                fPassword = value;
            }
        }
        private String fPassword;
    }

    public class UserManager : Hashtable, IFtpUserManager
    {
        public UserManager()
            : base()
        {
        }

        public virtual Boolean CheckIP(EndPoint remote, EndPoint local)
        {
            return true;
        }

        public virtual Boolean CheckLogin(String username, String password, VirtualFtpSession session)
        {
            User lUser = (User)this[username.ToLower()];

            if (lUser == null)
                return false;

            if (lUser.Password != password)
                return false;

            return true;
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
