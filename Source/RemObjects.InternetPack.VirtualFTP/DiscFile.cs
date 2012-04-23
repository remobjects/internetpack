using System;
using System.IO;
using RemObjects.InternetPack;

namespace RemObjects.InternetPack.Ftp.VirtualFtp
{
	/// <summary>
	/// Summary description for DiscFile.
	/// </summary>
	public class DiscFile : FtpFile
	{
    public DiscFile(IFtpFolder aParent, string aName, string aLocalPath) : base(aParent, aName)
    {
      fLocalPath = aLocalPath;
      WorldRead = true;
    }

    private string fLocalPath;    public string LocalPath {	get { return fLocalPath; } }

    public override DateTime Date     {       get { return File.GetLastWriteTime(LocalPath); }     }    public override int Size     {       get { return 5; }       set { /* no-op */ }     }    
    const int BUFFER_SIZE = 64*1024;

    public override void GetFile(Stream aToStream)    {      if (!File.Exists(LocalPath))
        throw new Exception("Error retrieving file from disk: file does not exist.");
      
      Stream lStream = new FileStream(LocalPath, FileMode.Open, FileAccess.Read, FileShare.Read); 
      try
      {
        byte[] lBuffer = new byte[BUFFER_SIZE];
        
        int lBytesRead = lStream.Read(lBuffer, 0, BUFFER_SIZE);
        while (lBytesRead > 0)        {          aToStream.Write(lBuffer,0,lBytesRead);          lBytesRead = lStream.Read(lBuffer, 0, BUFFER_SIZE);
        }
      }
      finally
      {
        lStream.Close();
      }
    }    public override void CreateFile(Stream aStream)    {      if (File.Exists(LocalPath))
        throw new Exception("Error savinf file to disk: file already exist.");
      
      Stream lStream = new FileStream(LocalPath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
      try
      {

        byte[] lBuffer = new byte[BUFFER_SIZE];
        
        int lBytesRead = aStream.Read(lBuffer, 0, BUFFER_SIZE);
        while (lBytesRead > 0)        {          lStream.Write(lBuffer,0,lBytesRead);          Size += lBytesRead;          lBytesRead = aStream.Read(lBuffer, 0, BUFFER_SIZE);
        }
        /* ToDo: eliminate one call to receive by checking for lBytesRead == BUFFER_SIZE? */
        lStream.Close();
        Complete = true;
      }
      catch (ConnectionClosedException)
      {
        lStream.Close();
        Complete = true;
      }
      catch (Exception)
      {
        lStream.Close();
        File.Delete(LocalPath);
        throw;
      }

    }
	}
}
