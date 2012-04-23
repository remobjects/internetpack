using System;
using System.Collections;
using System.Net;
using RemObjects.InternetPack;

namespace RemObjects.InternetPack.Ftp.VirtualFtp
{
  public class User
  {
    public User(string aUsername, string aPassword)    {    	fUsername = aUsername;      fPassword = aPassword;    }

    private string fUsername;    public string Username    {      get { return fUsername; }      set	{ fUsername = value; }    }

    private string fPassword;    public string Password    {      get { return fPassword; }      set	{ fPassword = value; }    }
  }

  public class UserManager: Hashtable, IFtpUserManager
	{
		public UserManager(): base ()
		{
		}

    public virtual bool CheckIP(EndPoint aRemote, EndPoint aLocal)
    {
      return true;
    }

    public virtual bool CheckLogin(string aUsername, string aPassword, VirtualFtpSession aSession)
    {
      User lUser = (User)this[aUsername.ToLower()];
      if (lUser == null) return false;
      if (lUser.Password != aPassword) return false;
      /* ToDo: add additional checks */
      return true;
    }

    public void AddUser(string aUsername, string aPassword)
    {
      Add(aUsername.ToLower(),new User(aUsername, aPassword));
    }

    public void AddUser(User aUser)
    {
      Add(aUser.Username.ToLower(), aUser);
    }

	}
}
