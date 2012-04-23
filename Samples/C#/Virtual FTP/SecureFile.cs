using System;
using System.IO;
using RemObjects.InternetPack.Core;

namespace RemObjects.InternetPack.Ftp.VirtualFtp
{

  public class SecureFile : FtpFile
  {
    public SecureFile(IFtpFolder aParent, string aName, SecureStorage aStorage) : base(aParent, aName)    {      fStorage = aStorage;      SecureFileName = fStorage.GetNewFilename();      WorldRead = true;    }    private SecureStorage fStorage;    private int fSize;    public override int Size { get { return fSize; } set { fSize = value; } }    private string fSecureFileName;    public string SecureFileName    {      get { return fSecureFileName; }      set	{ fSecureFileName = value; }    }    public override void Invalidate()
    {
      base.Invalidate ();
      File.Delete(fSecureFileName);
    }
    public override void GetFile(Stream aToStream)    {      if (!File.Exists(SecureFileName))
        throw new Exception("Error retrieving file from secure storage: file does not exist.");
      
      Stream lStream = fStorage.GetFile(SecureFileName, 0); 
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

      //return new FileStream(SecureFileName,FileMode.Open, FileAccess.Read, FileShare.Read);
    }    const int BUFFER_SIZE = 64*1024;

    public override void CreateFile(Stream aStream)    {      if (File.Exists(SecureFileName))
        throw new Exception("Error adding file to secure storage: file already exist.");
      
      //Stream lStream = new FileStream(SecureFileName,FileMode.CreateNew, FileAccess.Write, FileShare.None);
      Stream lStream = fStorage.CreateFile(SecureFileName); 
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
        File.Delete(SecureFileName);
        //lSession.CurrentFolder.DeleteFile(ea.FileName,lSession);
        throw;
      }

    }  }

}
