/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

using System;
using System.IO;

namespace RemObjects.InternetPack.Ftp.VirtualFtp
{
	/// <summary>
	/// Summary description for DiscFile.
	/// </summary>
	public class DiscFile : FtpFile
	{
		public DiscFile(IFtpFolder parent, String name, String localPath)
			: base(parent, name)
		{
			this.fLocalPath = localPath;
			this.WorldRead = true;
		}

		public String LocalPath
		{
			get
			{
				return fLocalPath;
			}
		}
		private readonly String fLocalPath;

		public override DateTime Date
		{
			get
			{
				return File.GetLastWriteTime(LocalPath);
			}
		}

		public override Int32 Size
		{
			get
			{
				return 5;
			}
			set
			{
				/* no-op */
			}
		}

		const Int32 BUFFER_SIZE = 64 * 1024;

		public override void GetFile(Stream destination)
		{
			if (!File.Exists(LocalPath))
				throw new Exception("Error retrieving file from disk: file does not exist.");

			Stream lStream = new FileStream(LocalPath, FileMode.Open, FileAccess.Read, FileShare.Read);
			try
			{
				Byte[] lBuffer = new Byte[BUFFER_SIZE];

				Int32 lBytesRead = lStream.Read(lBuffer, 0, BUFFER_SIZE);
				while (lBytesRead > 0)
				{
					destination.Write(lBuffer, 0, lBytesRead);
					lBytesRead = lStream.Read(lBuffer, 0, BUFFER_SIZE);
				}
			}
			finally
			{
				lStream.Close();
			}
		}

		public override void CreateFile(Stream source)
		{
			if (File.Exists(LocalPath))
				throw new Exception("Error savinf file to disk: file already exist.");

			Stream lStream = new FileStream(LocalPath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
			try
			{

				Byte[] lBuffer = new Byte[BUFFER_SIZE];

				Int32 lBytesRead = source.Read(lBuffer, 0, BUFFER_SIZE);
				while (lBytesRead > 0)
				{
					lStream.Write(lBuffer, 0, lBytesRead);
					Size += lBytesRead;
					lBytesRead = source.Read(lBuffer, 0, BUFFER_SIZE);
				}

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