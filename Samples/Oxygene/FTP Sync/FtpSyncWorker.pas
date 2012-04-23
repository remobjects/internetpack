namespace FtpSync;

interface

uses
  System.IO,
  System.Collections.Generic,
  System.Collections.Specialized,
  System.Text.RegularExpressions,
  RemObjects.InternetPack.Ftp;

type
  SyncMode = public enum(Local, Remote);

  FtpSyncWorker = public class
  private
    fLocalDirectory : string;
    fServer: String;
    fServerDirectory: String;
    fUsername: string;
    fPassword: string;
    fDoNotDeleteMissingItems: boolean;
    fRecursiveSync: boolean;
    fPassiveMode: boolean;    
    fDoShowFtpClientLog: boolean;
    fSyncMode: SyncMode := SyncMode.Local; 
    fFtpClient: FtpClient; 
    method fFtpClient_OnTransferProgress(aSender: object; ea: RemObjects.InternetPack.Events.TransferProgressEventArgs);
    method SetLocalDirectory(value: String);
    method SetServer(value: String);
    method SetServerDirectory(value: String);
    method SetDoNotDeleteMissingItems(value: boolean);    
    method SetRecursiveSync(value: Boolean);
    method SetPassiveMode(value: Boolean);

    method ServerLog(Sender: object; ea: RemObjects.InternetPack.CommandBased.ClientLogArgs);
    method ClientLog(aMessage: String);
  protected
  public
    property LocalDirectory: String read fLocalDirectory write SetLocalDirectory;
    property Server: String read fServer write SetServer;
    property ServerDirectory: String read fServerDirectory write SetServerDirectory;
    property DeleteMissing: boolean read fDoNotDeleteMissingItems write SetDoNotDeleteMissingItems;
    property Subdirectories: Boolean read fRecursiveSync write SetRecursiveSync;
    property Passive: Boolean read fPassiveMode write SetPassiveMode;
    
    method Log(Sender: object; ea: RemObjects.InternetPack.CommandBased.ClientLogArgs);
    method CheckArgs(args: array of string): Boolean;
    method SyncDirectory(aLocalDirectory: string; aRemoteDirectory: string);
    method Sync();
  end;
  
implementation

method FtpSyncWorker.ServerLog(Sender: object; ea: RemObjects.InternetPack.CommandBased.ClientLogArgs);
begin
	if (fDoShowFtpClientLog) then begin
    var lIcon: String := ' ';
		case ea.Direction of
		  RemObjects.InternetPack.CommandBased.LogDirection.Receive: lIcon := '<';
			RemObjects.InternetPack.CommandBased.LogDirection.Send:	lIcon := '>';
			RemObjects.InternetPack.CommandBased.LogDirection.Status:	lIcon := '!';
		end;		
				
		Console.WriteLine('[{0}] {1}', lIcon, ea.Text);
  end;
end;

method FtpSyncWorker.ClientLog(aMessage: String);
begin
  Console.WriteLine('[i] {0}', aMessage);
end;

method FtpSyncWorker.fFtpClient_OnTransferProgress(aSender: object; ea: RemObjects.InternetPack.Events.TransferProgressEventArgs);
begin
  Console.Write('.');
end;


method FtpSyncWorker.SetLocalDirectory(value: String);
begin
  if (fLocalDirectory = value) then Exit;       
  fLocalDirectory := value;
end;

method FtpSyncWorker.SetServer(value: String);
begin
  if (fServer = value) then Exit;       
  fServer := value;
end;

method FtpSyncWorker.SetServerDirectory(value: String);
begin
  if (fServerDirectory = value) then Exit;       
  fServerDirectory := value;
end;

method FtpSyncWorker.SetDoNotDeleteMissingItems(value: Boolean);
begin
  if (fDoNotDeleteMissingItems = value) then Exit;       
  fDoNotDeleteMissingItems := value;
end;

method FtpSyncWorker.SetRecursiveSync(value: Boolean);
begin
  if (fRecursiveSync = value) then Exit;       
  fRecursiveSync := value;
end;

method FtpSyncWorker.SetPassiveMode(value: Boolean);
begin
  if (fPassiveMode = value) then Exit;       
  fPassiveMode := value;
end;

method FtpSyncWorker.Log(Sender: object; ea: RemObjects.InternetPack.CommandBased.ClientLogArgs);
begin
  if (fDoShowFtpClientLog) then Console.WriteLine(ea.Text);
end;

method FtpSyncWorker.CheckArgs(args: array of string): Boolean;
begin
  var lCount: Integer := args.Length;
  var lBadParam: Boolean := false;
  if (lCount > 0) then
     fLocalDirectory := args[0]
  else
    lBadParam := true;

  if (lCount > 1) then begin
     var lMatch: Match := Regex.Match(args[1], '(?<user>\S+):(?<pass>\S+)@(?<server>[^/\s]+)/(?<dir>\S*)');
     if (lMatch.Success) then begin
        fUsername := lMatch.Groups['user'].Value;
        fPassword := lMatch.Groups['pass'].Value;
        fServer := lMatch.Groups['server'].Value;
        fServerDirectory := lMatch.Groups['dir'].Value;
     end else begin
        lBadParam := true;
        Console.WriteLine('Invalid server parameters');
        Console.WriteLine('');
     end;
  end else
      lBadParam := true;

  for i: Integer := 2 to lCount - 1 do begin
    case args[i].ToLower() of
      '-local': fSyncMode := SyncMode.Local;
      '-remote': fSyncMode := SyncMode.Remote;
      '-nodelete': fDoNotDeleteMissingItems := true;              
      '-passive': fPassiveMode := true;
      '-r': fRecursiveSync := true;
      '-l': fDoShowFtpClientLog := true;
      '-help': lBadParam := true;
      '/help': lBadParam := true;
      else begin
         Console.WriteLine('Invalid command line paramter ''+args[i]+''');
         Console.WriteLine('');
         lBadParam := true;
      end;
    end;
  end;

  if (lBadParam) then begin
    Console.WriteLine('RemObjects Internet Pack - FtpSync Sample');
		Console.WriteLine('');
		Console.WriteLine('  Usage: FtpSync local user:pass@server/remote [-local] [-remote] [-nodelete] [-passive] [-l] [-r]');
		Console.WriteLine();
		Console.WriteLine('  Compares files on local computer against those on the remote server, ');
		Console.WriteLine('  downloading those that are new or have changed.');
		Console.WriteLine();
		Console.WriteLine('  local     : local directory - long file names with spaces should be quoted');
		Console.WriteLine('  user/pass : ftp username & password');
		Console.WriteLine('  server    : ftp server (i.e., ftp.whatever.com');
		Console.WriteLine('  remote    : directory on remote server');
		Console.WriteLine();
		Console.WriteLine('  -nodelete : forbid to delete missing files');
		Console.WriteLine('  -passive  : turn on ''Passive mode''');
		Console.WriteLine('  -r        : recursive synchronization (through subdirectories)');
		Console.WriteLine('  -l        : log - show FTP commands');
		Console.WriteLine('  -local    : Local mode of sync (Master = ftp; slave = localhost). By default');
		Console.WriteLine('  -remote   : Remote mode of sync (Master = localhost; slave = ftp)');
		Console.WriteLine();
		Console.WriteLine('Press enter to exit.');
		Console.ReadLine();
  end;
      exit (not lBadParam);
  end;

method FtpSyncWorker.Sync();
begin     
  try
    fFtpClient := new FtpClient();
    fFtpClient.HostName := fServer;
    fFtpClient.UserName := fUsername;
    fFtpClient.Password := fPassword;
    fFtpClient.Passive := fPassiveMode;
    fFtpClient.Port := 21;

    fFtpClient.OnLog += new RemObjects.InternetPack.CommandBased.ClientLogEvent(ServerLog);
    fFtpClient.OnTransferProgress +=new RemObjects.InternetPack.Events.TransferProgressEventHandler(fFtpClient_OnTransferProgress);
    ClientLog('Connecting to ' + fServer);
    fFtpClient.Open();
    try
      fFtpClient.Login();
      SyncDirectory(fLocalDirectory, '/' + fServerDirectory);
    finally
      ClientLog('Disconnecting');
      fFtpClient.Quit();
      fFtpClient.Close();
    end
  except
    on ex: Exception do begin
      	ClientLog(String.Format('Error syncing directory ({0})', ex.Message));
        if (Assigned(ex.StackTrace) and fDoShowFtpClientLog) then ClientLog(ex.StackTrace);
        
    end;
  end;
  ClientLog('Press enter to continue...');
	Console.ReadLine();
end;


method FtpSyncWorker.SyncDirectory(aLocalDirectory: string; aRemoteDirectory: string);
begin
  var lOriginalLocalDirectory: String := Directory.GetCurrentDirectory();
	var lOriginalRemoteDirectory: String := fFtpClient.GetCurrentDirectory();
      
  try
		{$REGION Step into synchronized directories}
    ClientLog(String.Format('Local change directory to {0}', aLocalDirectory));
		Directory.SetCurrentDirectory(aLocalDirectory);

		ClientLog(String.Format('Remote change directory to {0}', aRemoteDirectory));
		fFtpClient.ChangeDirectory(aRemoteDirectory);
    {$ENDREGION}
		
		{$REGION Retrieve content}
		ClientLog('Retrieving directory contents');
		fFtpClient.List();
		{$ENDREGION}

		{$REGION Get Local Folders List}
		var lLocalFolders: Dictionary<String, DirectoryInfo> := new Dictionary<String, DirectoryInfo>();
		if (fRecursiveSync) then begin
			for each lName: String in Directory.GetDirectories(aLocalDirectory) do begin
				var d: DirectoryInfo := new DirectoryInfo(lName);
				lLocalFolders.Add(d.Name, d);
			end;
		end;
		{$ENDREGION}

		{$REGION Get Local Files List}
		var lLocalFiles: Dictionary<String, FileInfo> := new Dictionary<String, FileInfo>();
		for each lName: String in Directory.GetFiles(aLocalDirectory) do begin
			var f: FileInfo := new FileInfo(lName);
			lLocalFiles.Add(f.Name, f);
		end;
		{$ENDREGION}

		{$REGION Get Remote Files and Directories List}
		var lRemoteFolders: Dictionary<String,  FtpListingItem> := new Dictionary<string,FtpListingItem>();
		var lRemoteFiles: Dictionary<String,  FtpListingItem> := new Dictionary<string,FtpListingItem>();
		for each lRemoteItem: FtpListingItem in fFtpClient.CurrentDirectoryContents do begin
			if lRemoteItem.Directory then begin
				if fRecursiveSync then begin
					if (lRemoteItem.FileName <> '..') then
						lRemoteFolders.Add(lRemoteItem.FileName, lRemoteItem);
				end;
			end
			else
				lRemoteFiles.Add(lRemoteItem.FileName, lRemoteItem);
		end;
		{$ENDREGION}

		{$REGION Synchronization...}
		case fSyncMode of 
			{$REGION  Local Synchronization. Master: ftp; slave: local;}
			SyncMode.Local: begin
				{$REGION Folders synchronization}
				for each lName: String in lRemoteFolders.Keys do begin
					ClientLog(String.Format('Synchronizing folder "{0}"...', lName));
					var lLocalItemName: String := Path.Combine(aLocalDirectory, lName);

					if (not Directory.Exists(lLocalItemName)) then 
            Directory.CreateDirectory(lLocalItemName);

					SyncDirectory(lLocalItemName, lName);

					ClientLog(String.Format('Folder "{0}" has been synchronized', lName));
					lLocalFolders.Remove(lName);
				end;

				{$REGION Delete local folders that doesn't exists on FTP}
				if (not fDoNotDeleteMissingItems) then begin
					for each toDelete: DirectoryInfo in lLocalFolders.Values do
						toDelete.Delete(true);//delete recursive

					lLocalFolders.Clear();
				end;
				{$ENDREGION}

				{$ENDREGION}

				{$REGION Files synchronization}
				for each lName: String in lRemoteFiles.Keys do begin
					var lRemoteItem: FtpListingItem := lRemoteFiles[lName];
					var lNeedSync: Boolean := true;
					if (lLocalFiles.ContainsKey(lName)) then begin
						var lLocalItem: FileInfo := lLocalFiles[lName];
						lNeedSync := (
							(lLocalItem.Length <> lRemoteItem.Size) OR
							(lLocalItem.LastWriteTime <> lRemoteItem.FileDate));
					end;
					ClientLog(
						String.Format(
							'File {0} {1}', 
							lRemoteItem.FileName,
							iif(lNeedSync, 'requires synchronization.', 'doesn''t require synchronization.')));
					if lNeedSync then begin
					  ClientLog(String.Format('Downloading {0}...', lRemoteItem.FileName));
						with lStream: Stream := File.Open(Path.Combine(aLocalDirectory, lName), FileMode.Create) do begin
							fFtpClient.Retrieve(lRemoteItem, lStream);
							lStream.Close();
							File.SetLastWriteTime(lRemoteItem.FileName, lRemoteItem.FileDate);
						end;
						ClientLog(String.Format('File {0} has been downloaded.', lRemoteItem.FileName));
					end;
					lLocalFiles.Remove(lName);
				end;

				{$REGION Delete local files that doesn't exists on FTP}
				if (not fDoNotDeleteMissingItems) then begin
					for each toDelete: FileInfo in lLocalFiles.Values do
					  toDelete.Delete();
					lLocalFiles.Clear();
				end;
				{$ENDREGION}

				{$ENDREGION}
			end; 
			{$ENDREGION}

			{$REGION Remote Synchronization.	Master: local; slave: ftp;}
			SyncMode.Remote: begin

				{$REGION Folders Synchronization.}
				for each lName: String in lLocalFolders.Keys do begin
					ClientLog(String.Format("Synchronizing folder '{0}'...", lName));
					var lLocalItemName: String := Path.Combine(aLocalDirectory, lName);

					if not lRemoteFolders.ContainsKey(lName) then fFtpClient.MakeDirectory(lName);
					SyncDirectory(lLocalItemName, lName);
					ClientLog(String.Format("Folder '{0}' has been synchronized", lName));
					lRemoteFolders.Remove(lName);
				end;
				{$REGION Delete FTP folders that doesn't exists locally}
				if not fDoNotDeleteMissingItems then begin
					for each toDelete: FtpListingItem in lRemoteFolders.Values do
						fFtpClient.RemoveDirectory(toDelete.FileName);
					lRemoteFolders.Clear();
				end;
				{$ENDREGION}
				{$ENDREGION}

				{$REGION Files Synchronization.}
				for each lName: String in lLocalFiles.Keys do begin
					var lLocalItemName: String := Path.Combine(aLocalDirectory, lName);

					var lNeedSync: Boolean := true;
					if lRemoteFiles.ContainsKey(lName) then begin
						var lLocalItem: FileInfo := new FileInfo(lLocalItemName);
						var lRemoteItem: FtpListingItem := lRemoteFiles[lName];
						lNeedSync := (
							(lLocalItem.Length <> lRemoteItem.Size) OR
							(lLocalItem.LastWriteTime <> lRemoteItem.FileDate));
					end;
					ClientLog(
						String.Format(
							'File {0} {1}', 
							lName,
							iif(lNeedSync, 'requires synchronization.', 'doesn''t require synchronization.')));
					if lNeedSync then begin
						ClientLog(String.Format('Uploading {0}...', lName));								
						if (File.Exists(lLocalItemName)) then
							with fs: FileStream := new FileStream(lLocalItemName, FileMode.Open, FileAccess.Read) do 
								fFtpClient.Store(lName, fs);
							
						ClientLog(String.Format('File {0} has been uploaded.', lName));
					end;
					lRemoteFiles.Remove(lName);
				end;

				{$REGION Delete FTP files that doesn't exist locally}
				if not fDoNotDeleteMissingItems then begin
					for each toDelete: FtpListingItem in lRemoteFiles.Values do
						fFtpClient.Delete(toDelete.FileName);
					lRemoteFiles.Clear();
				end;
				{$ENDREGION}

				{$ENDREGION}
			end;
			{$ENDREGION}
		end;//case
		{$ENDREGION}
  finally
		{$REGION Step out of synchronized directories}
    Directory.SetCurrentDirectory(lOriginalLocalDirectory);
		ClientLog(String.Format('Local change directory to {0}', lOriginalLocalDirectory));
		fFtpClient.ChangeDirectory(lOriginalRemoteDirectory);
		ClientLog(String.Format('Remote change directory to {0}', lOriginalRemoteDirectory));
    {$ENDREGION}
  end;		
end;



end.