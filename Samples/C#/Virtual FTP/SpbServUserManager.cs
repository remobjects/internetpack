using System;
using System.Net;
using System.Text;
using System.Security.Cryptography;
using RemObjects.InternetPack.Ftp.VirtualFtp;
using ByteFX.Data.MySqlClient;

namespace spbServ.VirtualFtp
{
	/// <summary>
	/// Summary description for SpbServUserManager.
	/// </summary>
	public class SpbServUserManager : IFtpUserManager 
	{
    public SpbServUserManager()
    {
    }

    //private MySqlConnection fConnection;

    public static string GenerateHash(string aValue)
    {
      MD5 lMD5 = new MD5CryptoServiceProvider();
      byte[] lHash = lMD5.ComputeHash(Encoding.ASCII.GetBytes(aValue));
      StringBuilder sb = new StringBuilder(lHash.Length*1);
      for (int i = 0; i < lHash.Length; i++)
      {
        if (lHash[i] <= 0x0f) sb.Append('0');
        sb.Append(lHash[i].ToString("x"));
      }
      return sb.ToString();
    }

    const string sConnectionString = "user id=iptable;database=iptable;data source=ws9.elitedev.com;password=ipiv73737";

    public static MySqlConnection GetConnection()
    {
      MySqlConnection lConnection = new MySqlConnection();
      lConnection.ConnectionString = sConnectionString;
      lConnection.Open();
      return lConnection;
    }

    public static void ReleaseConnection(MySqlConnection e)
    {
      e.Close();
    }

    public virtual bool CheckIP(EndPoint aRemote, EndPoint aLocal)
    {
      using (MySqlConnection lConnection = new MySqlConnection())
      {
        lConnection.ConnectionString = sConnectionString;
        lConnection.Open();

        MySqlCommand lCmd = new MySqlCommand();
        lCmd.Connection = lConnection;

        lCmd.CommandText = String.Format("SELECT COUNT(*) FROM ips WHERE ip='{0}'",((IPEndPoint)aRemote).Address.ToString());
        object lValue = lCmd.ExecuteScalar();
        return ((int)lValue >= 1);
      }
    }

    public virtual bool CheckLogin(string aUsername, string aPassword, VirtualFtpSession aSession)
    {
      VirtualFtpSession lSession = (VirtualFtpSession)aSession;
      using (MySqlConnection lConnection = new MySqlConnection())
      {
        lConnection.ConnectionString = sConnectionString;
        lConnection.Open();

        MySqlCommand lCmd = new MySqlCommand(String.Format("select l.admin from login l inner join ips z on l.username=z.username where l.username='{0}' and z.ip='{1}' and l.passwordhash='{2}'",
        aUsername.Replace("'", "''"), ((IPEndPoint)aSession.RemoteEndPoint).Address.ToString(), GenerateHash(aPassword)), lConnection);
        using (MySqlDataReader lReader = lCmd.ExecuteReader())
        {
          if (lReader.Read())
          {
            if (lReader.GetInt32(0) == 1)
            {
              lSession.IsSuperUser = false;
              lSession.IsFileAdmin = true;
            } 
            else 
              if (lReader.GetInt32(0) == 2)
            {
              lSession.IsSuperUser = true;
              lSession.IsFileAdmin = true;
            } 
            else 
            {
              lSession.IsSuperUser = false;
              lSession.IsFileAdmin = false;
            }
            return true;
          } 
          else 
          {
            return false;
          }
        }
      }
    }

	}
}
