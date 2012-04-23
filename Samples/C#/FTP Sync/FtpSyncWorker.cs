using System;
using System.IO;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using RemObjects.InternetPack.Ftp;
using System.Collections.Generic;

namespace FtpSync
{	
	/// <summary>
	/// Summary description for FtpSyncWorker.
	/// </summary>
	public class FtpSyncWorker
	{
		public enum SyncMode {Local, Remote}

		public FtpSyncWorker()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		#region private fields...
		private string fLocalDirectory = "";
		private string fServer = "";
		private string fServerDirectory = "";
		private string fUserName = "";
		private string fPassword = "";
		private bool fDoNotDeleteMissingItems = false;
		private bool fRecursiveSync = false;
		private bool fPassiveMode = false;    
		private bool fDoShowFtpClientLog = false;
		private SyncMode fSyncMode = SyncMode.Local; 

		private FtpClient fFtpClient; 
		#endregion

		public void ServerLog(object Sender, RemObjects.InternetPack.CommandBased.ClientLogArgs ea)
		{
			if (fDoShowFtpClientLog)
			{
				String lIcon = " ";
				switch (ea.Direction)
				{
					case RemObjects.InternetPack.CommandBased.LogDirection.Receive: 
						lIcon = "<";
						break;
					case RemObjects.InternetPack.CommandBased.LogDirection.Send:
						lIcon = ">";
						break;
					case RemObjects.InternetPack.CommandBased.LogDirection.Status:
						lIcon = "!";
						break;
				}
				
				Console.WriteLine("[{0}] {1}", lIcon, ea.Text);
			}
		}
		public void ClientLog(String aMessage)
		{
			Console.WriteLine("[i] {0}", aMessage);
		}

		private void _Ftp_OnTransferProgress(object aSender, RemObjects.InternetPack.Events.TransferProgressEventArgs ea)
		{
			Console.Write(".");
		}

		public void Sync()
		{     
			try
			{
				fFtpClient = new FtpClient();
				fFtpClient.HostName = fServer;
				fFtpClient.UserName = fUserName;
				fFtpClient.Password = fPassword;
				fFtpClient.Passive = fPassiveMode;
				fFtpClient.Port = 21;

				fFtpClient.OnLog += new RemObjects.InternetPack.CommandBased.ClientLogEvent(ServerLog);
				fFtpClient.OnTransferProgress +=new RemObjects.InternetPack.Events.TransferProgressEventHandler(_Ftp_OnTransferProgress);
				ClientLog("Connecting to " + fServer);
				fFtpClient.Open();
				try
				{
					fFtpClient.Login();

					SyncDirectory(fLocalDirectory, "/" + fServerDirectory);
				}
				finally
				{
					ClientLog("Disconnecting");
					fFtpClient.Quit();
					fFtpClient.Close();
				}
				
			}
			catch (Exception ex)
			{
				ClientLog(String.Format("Error syncing directory ({0})", ex.Message));
				if (ex.StackTrace != null && fDoShowFtpClientLog)
				{
					ClientLog(ex.StackTrace);
				}
				
			}
			ClientLog("Press enter to continue...");
			Console.ReadLine();
		}

		public void SyncDirectory(string aLocalDirectory, string aRemoteDirectory)
		{
			string lOriginalLocalDirectory = Directory.GetCurrentDirectory();
			string lOriginalRemoteDirectory = fFtpClient.GetCurrentDirectory();
      
			try
			{
				#region Step into synchronized directories
				ClientLog(String.Format("Local change directory to {0}", aLocalDirectory));
				Directory.SetCurrentDirectory(aLocalDirectory);

				ClientLog(String.Format("Remote change directory to {0}", aRemoteDirectory));
				fFtpClient.ChangeDirectory(aRemoteDirectory);
				#endregion

				#region Retrieve content
				ClientLog("Retrieving directory contents");
				fFtpClient.List();
				#endregion

				#region Get Local Folders List
				Dictionary<String, DirectoryInfo> lLocalFolders = new Dictionary<String, DirectoryInfo>();
				if (fRecursiveSync)
				{
					foreach (String lName in Directory.GetDirectories(aLocalDirectory))
					{
						DirectoryInfo d = new DirectoryInfo(lName);
						lLocalFolders.Add(d.Name, d);
					}
				}
				#endregion

				#region Get Local Files List
				Dictionary<String, FileInfo> lLocalFiles = new Dictionary<String, FileInfo>();
				foreach (String lName in Directory.GetFiles(aLocalDirectory))
				{
					FileInfo f = new FileInfo(lName);
					lLocalFiles.Add(f.Name, f);
				}
				#endregion

				#region Get Remote Files and Directories List
				Dictionary<String,  FtpListingItem> lRemoteFolders = new Dictionary<string,FtpListingItem>();
				Dictionary<String,  FtpListingItem> lRemoteFiles = new Dictionary<string,FtpListingItem>();
				foreach(FtpListingItem lRemoteItem in fFtpClient.CurrentDirectoryContents)
				{
					if (lRemoteItem.Directory)
					{
						if (fRecursiveSync)
						{
							if (lRemoteItem.FileName != "..")
								lRemoteFolders.Add(lRemoteItem.FileName, lRemoteItem);
						}
					}
					else
						lRemoteFiles.Add(lRemoteItem.FileName, lRemoteItem);
				}
				#endregion

				#region Synchronization...
				switch (fSyncMode)
				{
					#region Local Synchronization. Master: ftp; slave: local;
					case SyncMode.Local:
					
						#region Folders synchronization
						foreach (String lName in lRemoteFolders.Keys)
						{

							ClientLog(String.Format("Synchronizing folder '{0}'...", lName));
							String lLocalItemName = Path.Combine(aLocalDirectory, lName);

							if (!Directory.Exists(lLocalItemName)) Directory.CreateDirectory(lLocalItemName);
							SyncDirectory(lLocalItemName, lName);
							ClientLog(String.Format("Folder '{0}' has been synchronized", lName));
							lLocalFolders.Remove(lName);
						}
						#region Delete local folders that doesn't exists on FTP
						if (!fDoNotDeleteMissingItems)
						{
							foreach (DirectoryInfo toDelete in lLocalFolders.Values)
							{
								toDelete.Delete(true);//delete recursive
							}
							lLocalFolders.Clear();
						}
						#endregion
						#endregion

						#region Files synchronization
						foreach (String lName in lRemoteFiles.Keys)
						{
							FtpListingItem lRemoteItem = lRemoteFiles[lName];
							Boolean lNeedSync = true;
							if (lLocalFiles.ContainsKey(lName))
							{
								FileInfo lLocalItem = lLocalFiles[lName];
								lNeedSync = (
									(lLocalItem.Length != lRemoteItem.Size) ||
									(lLocalItem.LastWriteTime != lRemoteItem.FileDate));
							}
							ClientLog(
								String.Format(
									"File {0} {1}", 
									lRemoteItem.FileName,
									lNeedSync ? "requires synchronization." : "doesn't require synchronization."));
							if (lNeedSync)
							{
								ClientLog(String.Format("Downloading {0}...", lRemoteItem.FileName));
								using (Stream lStream = File.Open(Path.Combine(aLocalDirectory, lName), FileMode.Create))
								{
									fFtpClient.Retrieve(lRemoteItem, lStream);
									lStream.Close();
									File.SetLastWriteTime(lRemoteItem.FileName, lRemoteItem.FileDate);
								}
								ClientLog(String.Format("File {0} has been downloaded.", lRemoteItem.FileName));
							}
							lLocalFiles.Remove(lName);
						}

						#region Delete local files that doesn't exists on FTP
						if (!fDoNotDeleteMissingItems)
						{
							foreach (FileInfo toDelete in lLocalFiles.Values)
							{
								toDelete.Delete();
							}
							lLocalFiles.Clear();
						}
						#endregion

						#endregion
						break; 
					#endregion

					#region Remote Synchronization.	Master: local; slave: ftp;
					case SyncMode.Remote:

						#region Folders Synchronization.	
						foreach (String lName in lLocalFolders.Keys)
						{
							ClientLog(String.Format("Synchronizing folder '{0}'...", lName));
							String lLocalItemName = Path.Combine(aLocalDirectory, lName);

							if (!lRemoteFolders.ContainsKey(lName)) fFtpClient.MakeDirectory(lName);
							SyncDirectory(lLocalItemName, lName);
							ClientLog(String.Format("Folder '{0}' has been synchronized", lName));
							lRemoteFolders.Remove(lName);
						}
						#region Delete FTP folders that doesn't exists locally
						if (!fDoNotDeleteMissingItems)
						{
							foreach (FtpListingItem toDelete in lRemoteFolders.Values)
							{
								fFtpClient.RemoveDirectory(toDelete.FileName);
							}
							lRemoteFolders.Clear();
						}
						#endregion
						#endregion

						#region Files Synchronization.
						foreach (String lName in lLocalFiles.Keys)
						{
							String lLocalItemName = Path.Combine(aLocalDirectory, lName);

							Boolean lNeedSync = true;
							if (lRemoteFiles.ContainsKey(lName))
							{
								FileInfo lLocalItem = new FileInfo(lLocalItemName);
								FtpListingItem lRemoteItem = lRemoteFiles[lName];
								lNeedSync = (
									(lLocalItem.Length != lRemoteItem.Size) ||
									(lLocalItem.LastWriteTime != lRemoteItem.FileDate));
							}
							ClientLog(
								String.Format(
									"File {0} {1}", 
									lName,
									lNeedSync ? "requires synchronization." : "doesn't require synchronization."));
							if (lNeedSync)
							{
								ClientLog(String.Format("Uploading {0}...", lName));								
								if (File.Exists(lLocalItemName))
									using (FileStream fs = new FileStream(lLocalItemName, FileMode.Open, FileAccess.Read))
									{
										fFtpClient.Store(lName, fs);
									}
								ClientLog(String.Format("File {0} has been uploaded.", lName));
							}
							lRemoteFiles.Remove(lName);
						}

						#region Delete FTP files that doesn't exist locally
						if (!fDoNotDeleteMissingItems)
						{
							foreach (FtpListingItem toDelete in lRemoteFiles.Values)
							{
								fFtpClient.Delete(toDelete.FileName);
							}
							lRemoteFiles.Clear();
						}
						#endregion

						#endregion
						break;
					#endregion
				}
				#endregion
			}
			finally
			{
				#region Step out of synchronized directories
				Directory.SetCurrentDirectory(lOriginalLocalDirectory);
				ClientLog(String.Format("Local change directory to {0}", lOriginalLocalDirectory));
				fFtpClient.ChangeDirectory(lOriginalRemoteDirectory);
				ClientLog(String.Format("Remote change directory to {0}", lOriginalRemoteDirectory));
				#endregion
			}

		}

		public bool CheckArgs(string[] args)
		{
			int lCount = args.Length;

			bool lBadParam = false;

			if (lCount > 0)
				fLocalDirectory = args[0];
			else
				lBadParam = true;

			if (lCount > 1)
			{
				Match lMatch = Regex.Match(args[1],@"(?<user>\S+):(?<pass>\S+)@(?<server>[^/\s]+)/(?<dir>\S*)");
				if (lMatch.Success)
				{
					fUserName = lMatch.Groups["user"].Value;
					fPassword = lMatch.Groups["pass"].Value;
					fServer = lMatch.Groups["server"].Value;
					fServerDirectory = lMatch.Groups["dir"].Value;
				}
				else
				{
					lBadParam = true;
					Console.WriteLine("Invalid server parameters");
					Console.WriteLine("");
				}
			}
			else
				lBadParam = true;

			for (int i = 2; i < lCount; i++)
			{
				switch (args[i].ToLower())
				{
					case "-local":
						fSyncMode = SyncMode.Local;
						break;

					case "-remote":
						fSyncMode = SyncMode.Remote;
						break;

					case "-nodelete":
						fDoNotDeleteMissingItems = true;              
						break;

					case "-passive":
						fPassiveMode = true;
						break;

					case "-r":
						fRecursiveSync = true;
						break;

					case "-l": 
						fDoShowFtpClientLog = true;
						break;

					case "-help": 
						lBadParam = true;
						break;
					case "/help": 
						lBadParam = true;
						break;

					default:
						Console.WriteLine("Invalid command line paramter \""+args[i]+"\"");
						Console.WriteLine("");
						lBadParam = true;
						break;
				}
			}

			if (lBadParam)
			{
				Console.WriteLine("RemObjects Internet Pack - FtpSync Sample");
				Console.WriteLine("");
				Console.WriteLine("  Usage: FtpSync directory user:pass@server/remote [-local] [-remote] [-nodelete] [-passive] [-l] [-r]");
				Console.WriteLine();
				Console.WriteLine("  Compares files on local computer against those on the remote server, ");
				Console.WriteLine("  downloading those that are new or have changed.");
				Console.WriteLine();
				Console.WriteLine("  directory     : local directory - long file names with spaces should be quoted");
				Console.WriteLine("  user/pass : ftp username & password");
				Console.WriteLine("  server    : ftp server (i.e., ftp.whatever.com");
				Console.WriteLine("  remote    : directory on remote server");
				Console.WriteLine();
				Console.WriteLine("  -nodelete : forbid to delete missing files");
				Console.WriteLine("  -passive  : turn on 'Passive mode'");
				Console.WriteLine("  -r        : recursive synchronization (through subdirectories)");
				Console.WriteLine("  -l        : log - show FTP commands");
				Console.WriteLine("  -local    : Local mode of sync (Master = ftp; slave = localhost). By default");
				Console.WriteLine("  -remote   : Remote mode of sync (Master = localhost; slave = ftp)");
				Console.WriteLine();
				Console.WriteLine("Press enter to exit.");
				Console.ReadLine();
			}
			return !lBadParam;
		}

		#region public properties...
		public string LocalDirectory
		{
			get
			{
				return fLocalDirectory;
			}
			set
			{
				if (fLocalDirectory == value)
					return;
				fLocalDirectory = value;
			}
		}
		public string Server
		{
			get
			{
				return fServer;
			}
			set
			{
				if (fServer == value)
					return;
				fServer = value;
			}
		}
		public string ServerDirectory
		{
			get
			{
				return fServerDirectory;
			}
			set
			{
				if (fServerDirectory == value)
					return;
				fServerDirectory = value;
			}
		}
		public bool DeleteMissing
		{
			get
			{
				return fDoNotDeleteMissingItems;
			}
			set
			{
				if (fDoNotDeleteMissingItems == value)
					return;
				fDoNotDeleteMissingItems = value;
			}
		}
		
		public bool Subdirectories
		{
			get
			{
				return fRecursiveSync;
			}
			set
			{
				if (fRecursiveSync == value)
					return;
				fRecursiveSync = value;
			}
		}
		public bool Passive
		{
			get
			{
				return fPassiveMode;
			}
			set
			{
				if (fPassiveMode == value)
					return;
				fPassiveMode = value;
			}
		}    
		#endregion
	}
}
