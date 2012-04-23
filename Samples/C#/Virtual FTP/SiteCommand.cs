using System;
using ByteFX.Data.MySqlClient;
using System.Collections;
using RemObjects.InternetPack.Core;
using RemObjects.InternetPack.Ftp;
using spbServ.VirtualFtp;
using RemObjects.InternetPack.Ftp.VirtualFtp;


namespace spbServ.VirtualFtp
{
	/// <summary>
	/// Summary description for SiteCommand.
	/// </summary>
  public class SiteCommand
  {
    private Hashtable fSubCommands = new Hashtable();

    public SiteCommand(FtpServer ftp) 
    {
      ftp.AddCustomCommand("SITE", new OnCommandHandler(Cmd_SITE));
      AddCustomCommand("ADDUSER", new OnCommandHandler(Cmd_Site_AddUser));
      AddCustomCommand("CHANGE", new OnCommandHandler(Cmd_Site_Change));
      AddCustomCommand("DELUSER", new OnCommandHandler(Cmd_Site_DelUser));
      AddCustomCommand("PASSWD", new OnCommandHandler(Cmd_Site_Passwd));
      AddCustomCommand("HELP", new OnCommandHandler(Cmd_Site_Help));
    }
    
    public void AddCustomCommand(String name, OnCommandHandler handler)
    {
      fSubCommands.Add(name, handler);
    }

    public static void Cmd_Site_AddUser(Object o, CommandEventArgs ea)
    {
      if (!((VirtualFtpSession)ea.Session).IsSuperUser) 
      {
        ea.Connection.WriteLine("500 Unknown command");
        return;
      }
      if (ea.Parameters.Length < 2)
      {
        ea.Connection.WriteLine("501 Syntax error in parameters or arguments.");
        return;
      }

      MySqlConnection lConnection = SpbServUserManager.GetConnection();
      try
      {
        MySqlCommand lCommand = lConnection.CreateCommand();
        lCommand.CommandText = String.Format("insert into login (username, passwordhash, admin) values ('{0}', '', 0)", ea.Parameters[1].Replace("'", "''"));
        try
        {
          lCommand.ExecuteNonQuery();
          ea.Connection.WriteLine("501 User Added.");
        } 
        catch 
        {
          ea.Connection.WriteLine("501 Unable to add user.");
        }
      }
      finally 
      {
        SpbServUserManager.ReleaseConnection(lConnection);
      }
    }

    public static void Cmd_Site_DelUser(Object o, CommandEventArgs ea)
    {
      if (!((VirtualFtpSession)ea.Session).IsSuperUser) 
      {
        ea.Connection.WriteLine("500 Unknown command");
        return;
      }
      if (ea.Parameters.Length < 2)
      {
        ea.Connection.WriteLine("501 Syntax error in parameters or arguments.");
        return;
      }
      if (ea.Parameters[1] == ((VirtualFtpSession)ea.Session).Username)
      {
        ea.Connection.WriteLine("501 You cannot delete yourself.");
        return;
      }
      MySqlConnection lConnection = SpbServUserManager.GetConnection();
      try
      {
        MySqlCommand lCommand = lConnection.CreateCommand();
        lCommand.CommandText = String.Format("delete from login where username='{0}'", ea.Parameters[1].Replace("'", "''"));
        try 
        {
          lCommand.ExecuteNonQuery();
          ea.Connection.WriteLine("501 User deleted.");
        }        
        catch 
        {
          ea.Connection.WriteLine("501 Unable to delete user.");
        }

      }
      finally 
      {
        SpbServUserManager.ReleaseConnection(lConnection);
      }
    }
    public static void Cmd_Site_Passwd(Object o, CommandEventArgs ea)
    {
      if (!((VirtualFtpSession)ea.Session).IsSuperUser) 
      {
        ea.Connection.WriteLine("500 Unknown command");
        return;
      }
      if (ea.Parameters.Length < 3)
      {
        ea.Connection.WriteLine("501 Syntax error in parameters or arguments.");
        return;
      }
      MySqlConnection lConnection = SpbServUserManager.GetConnection();
      try
      {
        MySqlCommand lCommand = lConnection.CreateCommand();
        lCommand.CommandText = String.Format("update login set passwordhash='{1}' where username='{0}'", ea.Parameters[1].Replace("'", "''"), SpbServUserManager.GenerateHash(ea.Parameters[2]));
        try
        {
          lCommand.ExecuteNonQuery();
          ea.Connection.WriteLine("501 Password changed.");
        }        
        catch 
        {
          ea.Connection.WriteLine("501 Unable to add user.");
        }
      }
      finally 
      {
        SpbServUserManager.ReleaseConnection(lConnection);
      }
    }

    public static void Cmd_Site_Change(Object o, CommandEventArgs ea)
    {
      if (!((VirtualFtpSession)ea.Session).IsSuperUser) 
      {
        ea.Connection.WriteLine("500 Unknown command");
        return;
      }
      if (ea.Parameters.Length < 4)
      {
        ea.Connection.WriteLine("501 Syntax error in parameters or arguments.");
        return;
      }
      String lField = ea.Parameters[2].ToUpper();
      String lValue = ea.Parameters[3];

      MySqlConnection lConnection = SpbServUserManager.GetConnection();
      try
      {
        try
        {
          MySqlCommand lCommand = lConnection.CreateCommand();
          if (lField.Equals("ADMIN"))
          {
            if (lValue.Equals("1"))
            {
              lCommand.CommandText = String.Format("update login set admin=2 where username='{0}'", ea.Parameters[1].Replace("'", "''"));
            } 
            else 
            {
              lCommand.CommandText = String.Format("update login set admin=0 where username='{0}'", ea.Parameters[1].Replace("'", "''"));
            }
            lCommand.ExecuteNonQuery();
          } 
          else if (lField.Equals("FILEADMIN"))
          {
            if (lValue.Equals("1"))
            {
              lCommand.CommandText = String.Format("update login set admin=1 where username='{0}'", ea.Parameters[1].Replace("'", "''"));
            } 
            else 
            {
              lCommand.CommandText = String.Format("update login set admin=0 where username='{0}'", ea.Parameters[1].Replace("'", "''"));
            }
            lCommand.ExecuteNonQuery();
          }
          ea.Connection.WriteLine("501 User changed.");
        }        
        catch 
        {
          ea.Connection.WriteLine("501 Unable to add user.");
        }
      }
      finally 
      {
        SpbServUserManager.ReleaseConnection(lConnection);
      }
    }

    public static void Cmd_Site_Help(Object o, CommandEventArgs ea)
    {
      if (!((VirtualFtpSession)ea.Session).IsSuperUser) 
      {
        ea.Connection.WriteLine("500 Unknown command");
        return;
      }
      ea.Connection.WriteLine(@"501-|---------------------------------------------------------------------|");
      ea.Connection.WriteLine(@"| Virtual FTP server                                                  |");
      ea.Connection.WriteLine(@"|---------------------------------------------------------------------|");
      ea.Connection.WriteLine(@"| ADDUSER username                  Add a new user to the user list   |");
      ea.Connection.WriteLine(@"| DELUSER username                  Remove a user from the user list  |");
      ea.Connection.WriteLine(@"| PASSWD username password          Change the password of a user     |");
      ea.Connection.WriteLine(@"| CHANGE username admin 1/0         Make a user a site admin          |");
      ea.Connection.WriteLine(@"| CHANGE username fileadmin 1/0     Make a user a file admin          |");
      ea.Connection.WriteLine(@"501 |---------------------------------------------------------------------|");
    }

    public void Cmd_SITE(Object o, CommandEventArgs ea)
    {
      if (((FtpSession)ea.Session).State != FtpState.LoggedIn)
      {
        ea.Connection.WriteLine("503 Bad sequence of commands.");
        return;
      }
      if (ea.Parameters.Length < 1)
      {
        ea.Connection.WriteLine("501 Syntax error in parameters or arguments.");
        return;
      }
      OnCommandHandler lCmd = (OnCommandHandler)fSubCommands[ea.Parameters[0].ToUpper()];
      if (lCmd == null)
      {
        ea.Connection.WriteLine("500 Unknown command");
        return;
      }
      lCmd(o, ea);
    }

	}
}
