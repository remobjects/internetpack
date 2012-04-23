using System;
using System.IO;
using System.Configuration;
using RemObjects.InternetPack.Ftp.VirtualFtp;
using RemObjects.DebugServer;

namespace VirtualFtp
{
	public class CmdMain
	{
    public static int Main(string[] args)
    {
      StartServer();
      Console.WriteLine("VirtualFTP running under "+Environment.OSVersion.ToString());
      Console.ReadLine();
      Console.WriteLine("Shutting down...");
      StopServer();
      Console.WriteLine("Down...");

      return 0;
    }

    static VirtualFolder fRootFolder;
    static IFtpUserManager fUserManager = null;
    static VirtualFtpServer fFtpServer;

    public static void StartServer()
    {
      string lDiskFolder = Path.GetDirectoryName(typeof(CmdMain).Assembly.Location)+@"\FtpRoot";;

      fRootFolder = new VirtualFolder(null, "[ROOT]");
      fRootFolder.Add(new VirtualFolder(null, "virtual"));
      fRootFolder.Add(new DiscFolder(null, "drive-c", @"c:\"));
      fRootFolder.Add(new DiscFolder(null, "disc", lDiskFolder));
      fRootFolder.Add(new EmptyFile(null, "=== Welcome to the FTP ==="));

      fUserManager = new UserManager();
      ((UserManager)fUserManager).AddUser("test", "test");

      fFtpServer = new VirtualFtpServer();
      fFtpServer.Port = 2222;
      fFtpServer.Timeout = 60*1000; /* 1 minute */
      fFtpServer.Binding.ListenerThreadCount = 10;
      fFtpServer.RootFolder = fRootFolder;
      fFtpServer.UserManager = fUserManager;
      fFtpServer.ServerName = "VirtualFTP Sample - powered by RemObjects Internet Pack for .NET";
      
      fFtpServer.Open();		

      Debug.Write("VirtualFTP 0.3 BETA - started up");
    }

    public static void StopServer()
    {
      fFtpServer.Close();
    }

  }
}
