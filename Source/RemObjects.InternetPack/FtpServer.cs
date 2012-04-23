/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET - Core Library
  (c)opyright RemObjects Software, LLC. 2003-2012. All rights reserved.

  Using this code requires a valid license of the RemObjects Internet Pack
  which can be obtained at http://www.remobjects.com?ip.
---------------------------------------------------------------------------*/

using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using RemObjects.InternetPack.CommandBased;

namespace RemObjects.InternetPack.Ftp
{
    // ftp://ftp.rfc-editor.org/in-notes/rfc959.txt
    #region Event Types
    public delegate void OnFtpUserLoginHandler(Object sender, FtpUserLoginEventArgs e);

    public delegate void OnFtpUserAccountHandler(Object sender, FtpUserAccountArgs e);

    public delegate void OnFtpChangeDirectoryHandler(Object sender, FtpChangeDirectoryArgs e);

    public delegate void OnFtpGetListingHandler(Object sender, FtpGetListingArgs e);

    public delegate void OnFtpRenameHandler(Object sender, FtpRenameEventArgs e);

    public delegate void OnFtpFileHandler(Object sender, FtpFileEventArgs e);

    public delegate void OnFtpTransferHandler(Object sender, FtpTransferEventArgs e);

    public class FtpUserLoginEventArgs : SessionEventArgs
    {
        public FtpUserLoginEventArgs(Object session, Connection connection, Server server)
            : base(session, connection, server)
        {
            this.LoginOk = false;
            this.NeedAccount = false;
        }

        public String UserName
        {
            get
            {
                return this.fUserName;
            }
            set
            {
                this.fUserName = value;
            }
        }
        private String fUserName;

        public String Password
        {
            get
            {
                return this.fPassword;
            }
            set
            {
                this.fPassword = value;
            }
        }
        private String fPassword;

        public Boolean LoginOk
        {
            get
            {
                return this.fLoginOk;
            }
            set
            {
                this.fLoginOk = value;
            }
        }
        private Boolean fLoginOk;

        public Boolean NeedAccount
        {
            get
            {
                return this.fNeedAccount;
            }
            set
            {
                this.fNeedAccount = value;
            }
        }
        private Boolean fNeedAccount;
    }

    public class FtpUserAccountArgs : SessionEventArgs
    {
        public FtpUserAccountArgs(Object session, Connection connection, Server server)
            : base(session, connection, server)
        {
        }

        public String AccountName
        {
            get
            {
                return this.fAccountName;
            }
            set
            {
                this.fAccountName = value;
            }
        }
        private String fAccountName;

        public Boolean LoginOk
        {
            get
            {
                return this.fLoginOk;
            }
            set
            {
                this.fLoginOk = value;
            }
        }
        private Boolean fLoginOk;
    }

    public class FtpChangeDirectoryArgs : SessionEventArgs
    {
        public FtpChangeDirectoryArgs(Object session, Connection connection, Server server)
            : base(session, connection, server)
        {
        }

        public String NewDirectory
        {
            get
            {
                return this.fNewDirectory;
            }
            set
            {
                this.fNewDirectory = value;
            }
        }
        private String fNewDirectory;

        public Boolean ChangeDirOk
        {
            get
            {
                return this.fChangeDirOk;
            }
            set
            {
                this.fChangeDirOk = value;
            }
        }
        private Boolean fChangeDirOk;
    }

    public class FtpGetListingArgs : SessionEventArgs
    {
        public FtpGetListingArgs(Object session, Connection connection, Server server)
            : base(session, connection, server)
        {
            this.fListing = new FtpListing();
        }

        public FtpListing Listing
        {
            get
            {
                return this.fListing;
            }
        }
        private FtpListing fListing;
    }

    public class FtpFileEventArgs : SessionEventArgs
    {
        public FtpFileEventArgs(Object session, Connection connection, Server server)
            : base(session, connection, server)
        {
        }

        public String FileName
        {
            get
            {
                return this.fFileName;
            }
            set
            {
                this.fFileName = value;
            }
        }
        private String fFileName;

        public Boolean Ok
        {
            get
            {
                return this.fOk;
            }
            set
            {
                this.fOk = value;
            }
        }
        private Boolean fOk;
    }

    public class FtpRenameEventArgs : FtpFileEventArgs
    {
        public FtpRenameEventArgs(Object session, Connection connection, Server server)
            : base(session, connection, server)
        {
        }

        public String NewFileName
        {
            get
            {
                return this.fNewFileName;
            }
            set
            {
                this.fNewFileName = value;
            }
        }
        private String fNewFileName;
    }

    public class FtpTransferEventArgs : FtpFileEventArgs
    {
        public FtpTransferEventArgs(Object session, Connection connection, Server server)
            : base(session, connection, server)
        {
        }

        public Boolean Append
        {
            get
            {
                return this.fAppend;
            }
            set
            {
                this.fAppend = value;
            }
        }
        private Boolean fAppend;

        public Int64 RestartPoint
        {
            get
            {
                return this.fRestartPoint;
            }
            set
            {
                this.fRestartPoint = value;
            }
        }
        private Int64 fRestartPoint;

        public Stream DataChannel
        {
            get
            {
                return this.fDataChannel;
            }
            set
            {
                this.fDataChannel = value;
            }
        }
        private Stream fDataChannel;
    }
    #endregion

#if DESIGN
    [System.Drawing.ToolboxBitmap(typeof(RemObjects.InternetPack.Server), "Glyphs.FtpServer.bmp")]
#endif
    public class FtpServer : CommandBasedServer
    {
        public FtpServer()
        {
            this.Port = 21;
            this.ServerName = "FTP Server Ready";
            this.QuitReply = "221 Bye";
            this.SystemType = "UNIX";
            this.AsciiEnter = "\r\n";
            this.Detailed500Errors = true;
        }

        #region Properties
        [DefaultValue("211 Bye"), Category("Server")]
        public String QuitReply
        {
            get
            {

                return this.fQuitReply;
            }
            set
            {
                this.fQuitReply = value;
            }
        }
        private String fQuitReply;

        [DefaultValue("UNIX"), Category("Server")]
        public String SystemType
        {
            get
            {
                return this.fSystemType;
            }
            set
            {
                this.fSystemType = value;
            }
        }
        private String fSystemType;

        public String AsciiEnter
        {
            get
            {
                return this.fAsciiEnter;
            }
            set
            {
                this.fAsciiEnter = value;
            }
        }
        private String fAsciiEnter;

        [Category("Server")]
        public String ServerName
        {
            get
            {
                return this.fServerName;
            }
            set
            {
                this.fServerName = value;
                this.Greeting = "220 " + value;
            }
        }
        private String fServerName;

        [DefaultValue(true), Category("Server")]
        public Boolean Detailed500Errors
        {
            get
            {
                return this.fDetailed500Errors;
            }
            set
            {
                this.fDetailed500Errors = value;
            }
        }
        private Boolean fDetailed500Errors;
        #endregion

        #region Events
        public event OnFtpUserLoginHandler OnUserLogin;
        public event OnFtpUserAccountHandler OnAccount;
        public event OnFtpChangeDirectoryHandler OnChangeDirectory;
        public event OnFtpGetListingHandler OnGetListing;
        public event OnFtpRenameHandler OnRename;
        public event OnFtpFileHandler OnDelete;
        public event OnFtpFileHandler OnMakeDirectory;
        public event OnFtpFileHandler OnDeleteDirectory;
        public event OnFtpTransferHandler OnStoreFile;
        public event OnFtpTransferHandler OnCanStoreFile;
        public event OnFtpTransferHandler OnRetrieveFile;
        public event OnFtpTransferHandler OnCanRetrieveFile;

        internal protected virtual void InvokeOnUserLogin(FtpUserLoginEventArgs e)
        {
            if (this.OnUserLogin != null)
                this.OnUserLogin(this, e);
        }

        internal protected virtual void InvokeOnAccount(FtpUserAccountArgs e)
        {
            if (this.OnAccount != null)
                this.OnAccount(this, e);
        }

        internal protected virtual void InvokeOnChangeDirectory(FtpChangeDirectoryArgs e)
        {
            if (this.OnChangeDirectory != null)
                this.OnChangeDirectory(this, e);
        }

        internal protected virtual void InvokeOnGetListing(FtpGetListingArgs e)
        {
            if (this.OnGetListing != null)
                this.OnGetListing(this, e);
        }

        internal protected virtual void InvokeOnRename(FtpRenameEventArgs e)
        {
            if (this.OnRename != null)
                this.OnRename(this, e);
        }

        internal protected virtual void InvokeOnDelete(FtpFileEventArgs e)
        {
            if (this.OnDelete != null)
                this.OnDelete(this, e);
        }

        internal protected virtual void InvokeOnMakeDirectory(FtpFileEventArgs e)
        {
            if (this.OnMakeDirectory != null)
                this.OnMakeDirectory(this, e);
        }

        internal protected virtual void InvokeOnDeleteDirectory(FtpFileEventArgs e)
        {
            if (this.OnDeleteDirectory != null)
                this.OnDeleteDirectory(this, e);
        }

        internal protected virtual void InvokeOnStoreFile(FtpTransferEventArgs e)
        {
            if (this.OnStoreFile != null)
                this.OnStoreFile(this, e);
        }

        internal protected virtual void InvokeOnRetrieveFile(FtpTransferEventArgs e)
        {
            if (this.OnRetrieveFile != null)
                this.OnRetrieveFile(this, e);
        }

        internal protected virtual void InvokeOnCanStoreFile(FtpTransferEventArgs e)
        {
            if (this.OnCanStoreFile != null)
                this.OnCanStoreFile(this, e);
        }

        internal protected virtual void InvokeOnCanRetrieveFile(FtpTransferEventArgs e)
        {
            if (this.OnCanRetrieveFile != null)
                this.OnCanRetrieveFile(this, e);
        }

        protected internal override void InvokeOnClientDisconnected(SessionEventArgs e)
        {
            base.InvokeOnClientDisconnected(e);

            FtpSession lSession = (FtpSession)e.Session;
            if (lSession.ActiveConnection != null)
            {
                try
                {
                    lSession.ActiveConnection.Close();
                }
                catch
                {
                }
                lSession.ActiveConnection = null;
            }

            if (lSession.PassiveServer != null)
            {
                try
                {
                    lSession.PassiveServer.Close();
                }
                catch
                {
                }
                lSession.PassiveServer = null;
            }

            if (lSession.TransferThread != null)
            {
                try
                {
                    lSession.TransferThread.Abort();
                }
                catch
                {
                }
                lSession.TransferThread = null;
            }
        }
        #endregion

        public static Boolean ValidFilename(String value)
        {
            if (value.IndexOfAny(new Char[] { '\\', '|', '>', '<', '*', '?', ':', '"' }) != -1)
                return false;

            return true;
        }

        public static String ValidateDirectory(String value)
        {
            StringBuilder lResult = new StringBuilder();
            lResult.Append('/');

            String[] lTemp = value.Split(new Char[] { '/' });
            for (Int32 i = lTemp.Length - 1; i >= 0; i--)
            {
                if (lTemp[i] == ".")
                {
                    lTemp[i] = "";

                    continue;
                }

                if (lTemp[i] == "..")
                {
                    lTemp[i] = "";

                    if (i > 0)
                        lTemp[i - 1] = "";

                    continue;
                }

                if (lTemp[i].IndexOfAny(new Char[] { '\\', '|', '>', '<', '*', '?', ':', '"' }) != -1)
                {
                    if (i > 0)
                        lTemp[i - 1] = "";
                }
            }

            for (Int32 i = 0; i < lTemp.Length; i++)
            {
                if (lTemp[i].Length == 0)
                    continue;

                lResult.Append(lTemp[i]);
                lResult.Append('/');
            }

            if (lResult.Length == 1)
                return lResult.ToString();

            String lTemp2 = lResult.ToString();
            return lTemp2.Substring(0, lTemp2.Length - 1); // remove the last /
        }

        protected override Type GetDefaultSessionClass()
        {
            return typeof(FtpSession);
        }

        protected override void InitCommands()
        {
            this.Commands.Add("QUIT", new OnCommandHandler(Cmd_QUIT));
            this.Commands.Add("NOOP", new OnCommandHandler(Cmd_NOOP));
            this.Commands.Add("USER", new OnCommandHandler(Cmd_USER));
            this.Commands.Add("ALLO", new OnCommandHandler(Cmd_NOOP));
            this.Commands.Add("PASS", new OnCommandHandler(Cmd_PASS));
            this.Commands.Add("ACCT", new OnCommandHandler(Cmd_ACCT));
            this.Commands.Add("CWD", new OnCommandHandler(Cmd_CWD));
            this.Commands.Add("CDUP", new OnCommandHandler(Cmd_CDUP));
            this.Commands.Add("PWD", new OnCommandHandler(Cmd_PWD));
            this.Commands.Add("SYST", new OnCommandHandler(Cmd_SYST));
            this.Commands.Add("TYPE", new OnCommandHandler(Cmd_TYPE));
            this.Commands.Add("PORT", new OnCommandHandler(Cmd_PORT));
            this.Commands.Add("REST", new OnCommandHandler(Cmd_REST));
            this.Commands.Add("LIST", new OnCommandHandler(Cmd_LIST));
            this.Commands.Add("PASV", new OnCommandHandler(Cmd_PASV));
            this.Commands.Add("RNFR", new OnCommandHandler(Cmd_RNFR));
            this.Commands.Add("RNTO", new OnCommandHandler(Cmd_RNTO));
            this.Commands.Add("DELE", new OnCommandHandler(Cmd_DELE));
            this.Commands.Add("RMD", new OnCommandHandler(Cmd_RMD));
            this.Commands.Add("MKD", new OnCommandHandler(Cmd_MKD));
            this.Commands.Add("STOR", new OnCommandHandler(Cmd_STOR));
            this.Commands.Add("APPE", new OnCommandHandler(Cmd_APPE));
            this.Commands.Add("RETR", new OnCommandHandler(Cmd_RETR));
            this.Commands.Add("ABOR", new OnCommandHandler(Cmd_ABOR));

            /* Aliases */
            this.Commands.Add("CD", new OnCommandHandler(Cmd_CWD));
        }

        /*
            STRU <SP> <structure-code> <CRLF> // only support file
            MODE <SP> <mode-code> <CRLF> // stream, maybe block if needed

            NLST [<SP> <pathname>] <CRLF> nlist
            SITE <SP> <String> <CRLF> site specific commands
            STAT [<SP> <pathname>] <CRLF> status
        */

        private static Boolean ValidateConnection(FtpSession session)
        {
            if (!session.Passive)
                return (session.ActiveConnection != null && session.ActiveConnection.Connected);

            if (session.PassiveServer == null)
                return false;

            try
            {
                Connection lConnection = session.PassiveServer.WaitForConnection();
                session.PassiveServer.Close();
                session.PassiveServer = null;
                session.ActiveConnection = lConnection;

                return true;
            }
            catch
            {
                session.PassiveServer = null;
                session.ActiveConnection = null;

                return false;
            }
        }

        public static void Cmd_NOOP(Object sender, CommandEventArgs e)
        {
            e.Connection.WriteLine("200 OK");
        }

        public static void Cmd_RNFR(Object sender, CommandEventArgs e)
        {
            FtpSession lSession = (FtpSession)e.Session;

            if (lSession.State == FtpState.LoggedIn)
            {
                if ((e.AllParameters.Length == 0) || (!ValidFilename(e.AllParameters)))
                {
                    e.Connection.WriteLine("501 Syntax error in parameters or arguments.");
                    return;
                }

                String lNewDir = e.AllParameters;

                if (lNewDir.StartsWith("/"))
                {
                    lNewDir = ValidateDirectory(lNewDir);
                }
                else
                {
                    String lTemp = lSession.Directory;
                    if (!lTemp.EndsWith("/"))
                        lTemp = lTemp + "/";
                    lNewDir = ValidateDirectory(lTemp + lNewDir);
                }
                lSession.RenameFrom = lNewDir;
                e.Connection.WriteLine("350 Ready for destination name.");
            }
            else
            {
                e.Connection.WriteLine("503 Bad sequence of commands.");
            }
        }

        public static void Cmd_RNTO(Object sender, CommandEventArgs e)
        {
            FtpSession lSession = (FtpSession)e.Session;

            if (lSession.State == FtpState.LoggedIn)
            {
                if ((e.AllParameters.Length == 0) || (!ValidFilename(e.AllParameters)))
                {
                    e.Connection.WriteLine("501 Syntax error in parameters or arguments.");
                    return;
                }

                if (lSession.RenameFrom == null)
                {
                    e.Connection.WriteLine("500 Bad sequence of commands.");
                    return;
                }

                String lNewDir = e.AllParameters;

                if (lNewDir.StartsWith("/"))
                {
                    lNewDir = ValidateDirectory(lNewDir);
                }
                else
                {
                    String lTemp = lSession.Directory;
                    if (!lTemp.EndsWith("/"))
                        lTemp = lTemp + "/";
                    lNewDir = ValidateDirectory(lTemp + lNewDir);
                }

                FtpRenameEventArgs lEventArgs = new FtpRenameEventArgs(e.Session, e.Connection, e.Server);
                lEventArgs.FileName = lSession.RenameFrom;
                lSession.RenameFrom = null;
                lEventArgs.NewFileName = lNewDir;
                try
                {
                    ((FtpServer)e.Server).InvokeOnRename(lEventArgs);
                }
                catch (FtpException ex)
                {
                    e.Connection.WriteLine(ex.ToString());
                    return;
                }
                catch
                {
                    e.Connection.WriteLine("500 Internal Error");
                    return;
                }

                if (lEventArgs.Ok)
                    e.Connection.WriteLine("250 Rename successful");
                else
                    e.Connection.WriteLine("550 Permission Denied");
            }
            else
            {
                e.Connection.WriteLine("503 Bad sequence of commands.");
            }
        }

        public static void Cmd_ABOR(Object sender, CommandEventArgs e)
        {
            FtpSession lSession = (FtpSession)e.Session;

            if (lSession.State == FtpState.Transfer && lSession.TransferThread != null)
            {
                lSession.TransferThread.Abort();
                lSession.TransferThread = null;
                e.Connection.WriteLine("250 ABOR successful");
            }
            else
            {
                e.Connection.WriteLine("503 Bad sequence of commands.");
            }
        }

        public static void Cmd_DELE(Object sender, CommandEventArgs e)
        {
            FtpSession lSession = (FtpSession)e.Session;

            if (lSession.State == FtpState.LoggedIn)
            {
                if ((e.AllParameters.Length == 0) || (!ValidFilename(e.AllParameters)))
                {
                    e.Connection.WriteLine("501 Syntax error in parameters or arguments.");
                    return;
                }
                String lNewDir = e.AllParameters;

                if (lNewDir.StartsWith("/"))
                {
                    lNewDir = ValidateDirectory(lNewDir);
                }
                else
                {
                    String lTemp = lSession.Directory;
                    if (!lTemp.EndsWith("/"))
                        lTemp = lTemp + "/";
                    lNewDir = ValidateDirectory(lTemp + lNewDir);
                }

                FtpFileEventArgs lEventArgs = new FtpFileEventArgs(e.Session, e.Connection, e.Server);
                lEventArgs.FileName = lNewDir;
                try
                {
                    ((FtpServer)e.Server).InvokeOnDelete(lEventArgs);
                }
                catch (FtpException ex)
                {
                    e.Connection.WriteLine(ex.ToString());
                    return;
                }
                catch
                {
                    e.Connection.WriteLine("500 Internal Error");
                    return;
                }

                if (lEventArgs.Ok)
                    e.Connection.WriteLine("250 Delete successful");
                else
                    e.Connection.WriteLine("550 Permission Denied");
            }
            else
            {
                e.Connection.WriteLine("503 Bad sequence of commands.");
            }
        }

        public static void Cmd_RMD(Object sender, CommandEventArgs e)
        {
            FtpSession lSession = (FtpSession)e.Session;

            if (lSession.State == FtpState.LoggedIn)
            {
                if ((e.AllParameters.Length == 0) || (!ValidFilename(e.AllParameters)))
                {
                    e.Connection.WriteLine("501 Syntax error in parameters or arguments.");
                    return;
                }

                String lNewDir = e.AllParameters;
                if (lNewDir.StartsWith("/"))
                {
                    lNewDir = ValidateDirectory(lNewDir);
                }
                else
                {
                    String lTemp = lSession.Directory;
                    if (!lTemp.EndsWith("/"))
                        lTemp = lTemp + "/";
                    lNewDir = ValidateDirectory(lTemp + lNewDir);
                }

                FtpFileEventArgs lEventArgs = new FtpFileEventArgs(e.Session, e.Connection, e.Server);
                lEventArgs.FileName = lNewDir;
                try
                {
                    ((FtpServer)e.Server).InvokeOnDeleteDirectory(lEventArgs);
                }
                catch (FtpException ex)
                {
                    e.Connection.WriteLine(ex.ToString());
                    return;
                }
                catch
                {
                    e.Connection.WriteLine("500 Internal Error");
                    return;
                }

                if (lEventArgs.Ok)
                    e.Connection.WriteLine("250 RMD command succesfully");
                else
                    e.Connection.WriteLine("550 Permission Denied");
            }
            else
            {
                e.Connection.WriteLine("503 Bad sequence of commands.");
            }
        }

        public static void Cmd_MKD(Object sender, CommandEventArgs e)
        {
            FtpSession lSession = (FtpSession)e.Session;

            if (lSession.State == FtpState.LoggedIn)
            {
                if ((e.AllParameters.Length == 0) || (!ValidFilename(e.AllParameters)))
                {
                    e.Connection.WriteLine("501 Syntax error in parameters or arguments.");
                    return;
                }

                String lNewDir = e.AllParameters;
                if (lNewDir.StartsWith("/"))
                {
                    lNewDir = ValidateDirectory(lNewDir);
                }
                else
                {
                    String lTemp = lSession.Directory;
                    if (!lTemp.EndsWith("/"))
                        lTemp = lTemp + "/";
                    lNewDir = ValidateDirectory(lTemp + lNewDir);
                }

                FtpFileEventArgs lEventArgs = new FtpFileEventArgs(e.Session, e.Connection, e.Server);
                lEventArgs.FileName = lNewDir;
                try
                {
                    ((FtpServer)e.Server).InvokeOnMakeDirectory(lEventArgs);
                }
                catch (FtpException ex)
                {
                    e.Connection.WriteLine(ex.ToString());
                    return;
                }
                catch
                {
                    e.Connection.WriteLine("500 Internal Error");
                    return;
                }

                if (lEventArgs.Ok)
                    e.Connection.WriteLine(String.Format("257 \"{0}\" - Directory successfully created", lEventArgs.FileName));
                else
                    e.Connection.WriteLine("550 Permission Denied");
            }
            else
            {
                e.Connection.WriteLine("503 Bad sequence of commands.");
            }
        }

        public static void Cmd_STOR(Object sender, CommandEventArgs e)
        {
            FtpSession lSession = (FtpSession)e.Session;

            if (lSession.State == FtpState.LoggedIn)
            {
                if ((e.AllParameters.Length == 0) || (!ValidFilename(e.AllParameters)))
                {
                    e.Connection.WriteLine("501 Syntax error in parameters or arguments.");
                    return;
                }

                String lNewDir = e.AllParameters;
                if (lNewDir.StartsWith("/"))
                {
                    lNewDir = ValidateDirectory(lNewDir);
                }
                else
                {
                    String lTemp = lSession.Directory;
                    if (!lTemp.EndsWith("/"))
                        lTemp = lTemp + "/";
                    lNewDir = ValidateDirectory(lTemp + lNewDir);
                }

                FtpTransferEventArgs lEventArgs = new FtpTransferEventArgs(e.Session, e.Connection, e.Server);
                lEventArgs.FileName = lNewDir;
                lEventArgs.Append = false;
                lEventArgs.RestartPoint = 0;
                lSession.RestartPoint = 0;

                try
                {
                    ((FtpServer)lEventArgs.Server).InvokeOnCanStoreFile(lEventArgs);
                }
                catch (FtpException ex)
                {
                    lEventArgs.Connection.WriteLine(ex.ToString());
                    return;
                }
                catch
                {
                    lEventArgs.Connection.WriteLine("500 Internal Error");
                    return;
                }

                if (lEventArgs.Ok)
                {
                    if (lSession.Passive)
                        e.Connection.WriteLine("125 Data connection already open; transfer starting");

                    if (!ValidateConnection(lSession))
                    {
                        e.Connection.WriteLine("425 Unable to build data connection");
                        return;
                    }

                    if (!lSession.Passive)
                        e.Connection.WriteLine("150 Opening data connection");
                }
                else
                {
                    lEventArgs.Connection.WriteLine("550 Permission Denied");
                    return;
                }
                lSession.State = FtpState.Transfer;

                if (lSession.TransferThread != null)
                {
                    lSession.TransferThread.Abort();
                    lSession.TransferThread = null;
                }
                lSession.TransferThread = new FtpTransferThread(lEventArgs, true);
            }
            else
            {
                e.Connection.WriteLine("503 Bad sequence of commands.");
            }
        }

        public static void Cmd_APPE(Object sender, CommandEventArgs e)
        {
            FtpSession lSession = (FtpSession)e.Session;

            if (lSession.State == FtpState.LoggedIn)
            {
                if ((e.AllParameters.Length == 0) || (!ValidFilename(e.AllParameters)))
                {
                    e.Connection.WriteLine("501 Syntax error in parameters or arguments.");
                    return;
                }

                String lNewDir = e.AllParameters;
                if (lNewDir.StartsWith("/"))
                {
                    lNewDir = ValidateDirectory(lNewDir);
                }
                else
                {
                    String lTemp = lSession.Directory;
                    if (!lTemp.EndsWith("/"))
                        lTemp = lTemp + "/";
                    lNewDir = ValidateDirectory(lTemp + lNewDir);
                }

                FtpTransferEventArgs lArgs = new FtpTransferEventArgs(e.Session, e.Connection, e.Server);
                lArgs.FileName = lNewDir;
                lArgs.Append = true;
                lArgs.RestartPoint = 0;
                lSession.RestartPoint = 0;

                try
                {
                    ((FtpServer)lArgs.Server).InvokeOnCanStoreFile(lArgs);
                }
                catch (FtpException ex)
                {
                    lArgs.Connection.WriteLine(ex.ToString());
                    return;
                }
                catch
                {
                    lArgs.Connection.WriteLine("500 Internal Error");
                    return;
                }

                if (lArgs.Ok)
                {
                    if (lSession.Passive)
                        e.Connection.WriteLine("125 Data connection already open; transfer starting");

                    if (!ValidateConnection(lSession))
                    {
                        e.Connection.WriteLine("425 Unable to build data connection");
                        return;
                    }
                    if (!lSession.Passive)
                        e.Connection.WriteLine("150 Opening data connection");
                }
                else
                {
                    lArgs.Connection.WriteLine("550 Permission Denied");
                    return;
                }
                lSession.State = FtpState.Transfer;

                if (lSession.TransferThread != null)
                {
                    lSession.TransferThread.Abort();
                    lSession.TransferThread = null;
                }
                lSession.TransferThread = new FtpTransferThread(lArgs, true);
            }
            else
            {
                e.Connection.WriteLine("503 Bad sequence of commands.");
            }
        }

        public static void Cmd_RETR(Object sender, CommandEventArgs e)
        {
            FtpSession lSession = (FtpSession)e.Session;

            if (lSession.State == FtpState.LoggedIn)
            {
                if ((e.AllParameters.Length == 0))
                {
                    e.Connection.WriteLine("501 Syntax error in parameters or arguments.");
                    return;
                }

                String lNewDir = e.AllParameters;
                if (lNewDir.StartsWith("/"))
                {
                    lNewDir = ValidateDirectory(lNewDir);
                }
                else
                {
                    String lTemp = lSession.Directory;
                    if (!lTemp.EndsWith("/"))
                        lTemp = lTemp + "/";
                    lNewDir = ValidateDirectory(lTemp + lNewDir);
                }

                FtpTransferEventArgs lEventArgs = new FtpTransferEventArgs(e.Session, e.Connection, e.Server);
                lEventArgs.FileName = lNewDir;
                lEventArgs.Append = false;
                lEventArgs.RestartPoint = lSession.RestartPoint;
                lEventArgs.DataChannel = lSession.ActiveConnection;
                lSession.RestartPoint = 0;

                try
                {
                    ((FtpServer)lEventArgs.Server).InvokeOnCanRetrieveFile(lEventArgs);
                }
                catch (FtpException ex)
                {
                    lEventArgs.Connection.WriteLine(ex.ToString());
                    return;
                }
                catch
                {
                    lEventArgs.Connection.WriteLine("500 Internal Error");
                    return;
                }

                if (lEventArgs.Ok)
                {
                    if (lSession.Passive)
                        e.Connection.WriteLine("125 Data connection already open; transfer starting");

                    if (!ValidateConnection(lSession))
                    {
                        e.Connection.WriteLine("425 Unable to build data connection");
                        return;
                    }

                    if (!lSession.Passive)
                        e.Connection.WriteLine("150 Opening data connection");

                    lSession.State = FtpState.Transfer;
                    if (lSession.TransferThread != null)
                    {
                        lSession.TransferThread.Abort();
                        lSession.TransferThread = null;
                    }
                    lSession.TransferThread = new FtpTransferThread(lEventArgs, false);
                }
                else
                {
                    lEventArgs.Connection.WriteLine("550 Permission Denied");
                }
            }
            else
            {
                e.Connection.WriteLine("503 Bad sequence of commands.");
            }
        }

        public static void Cmd_PASV(Object sender, CommandEventArgs e)
        {
            FtpSession lSession = (FtpSession)e.Session;

            if (lSession.State == FtpState.LoggedIn)
            {
                if (lSession.ActiveConnection != null)
                {
                    if (lSession.ActiveConnection.Connected)
                        lSession.ActiveConnection.Close();

                    lSession.ActiveConnection = null;
                }

                if (lSession.PassiveServer == null)
                {
                    lSession.PassiveServer = new SimpleServer();

                    lSession.PassiveServer.Binding.Address = ((IPEndPoint)e.Connection.LocalEndPoint).Address;
                    lSession.PassiveServer.Open();
                }
                lSession.Passive = true;

                Byte[] lAddress;
#if FULLFRAMEWORK
                lAddress = ((IPEndPoint)lSession.PassiveServer.Binding.ListeningSocket.LocalEndPoint).Address.GetAddressBytes();
#endif
#if COMPACTFRAMEWORK
                IPAddress lIPAddress = ((IPEndPoint)lSession.PassiveServer.Binding.ListeningSocket.LocalEndPoint).Address;
                String[] lIPAddressstr = lIPAddress.ToString().Split(new Char[] {'.'});
                lAddress = new Byte[lIPAddressstr.Length];
                for (Int32 i = 0; i < lIPAddressstr.Length; i++)
                    lAddress[i] = Byte.Parse(lIPAddressstr[i]);
#endif

                Int32 lPort = ((IPEndPoint)lSession.PassiveServer.Binding.ListeningSocket.LocalEndPoint).Port;
                e.Connection.WriteLine("227 Entering Passive Mode ({0},{1},{2},{3},{4},{5}).", lAddress[0], lAddress[1], lAddress[2], lAddress[3],
                    unchecked((Byte)(lPort >> 8)), unchecked((Byte)lPort));

            }
            else
            {
                e.Connection.WriteLine("503 Bad sequence of commands.");
            }
        }

        public static void Cmd_LIST(Object sender, CommandEventArgs e)
        {
            FtpSession lSession = (FtpSession)e.Session;

            if (lSession.State == FtpState.LoggedIn)
            {

                if (lSession.Passive)
                    e.Connection.WriteLine("125 Data connection already open; transfer starting");

                if (!ValidateConnection(lSession))
                {
                    e.Connection.WriteLine("425 Unable to build data connection");
                }
                else
                {
                    FtpGetListingArgs lListingArgs = new FtpGetListingArgs(e.Session, e.Connection, e.Server);
                    if (!lSession.Passive)
                        e.Connection.WriteLine("150 Opening data connection");
                    try
                    {
                        ((FtpServer)e.Server).InvokeOnGetListing(lListingArgs);
                    }
                    catch (FtpException ex)
                    {
                        e.Connection.WriteLine(ex.ToString());
                        return;
                    }

                    for (Int32 i = 0; i < lListingArgs.Listing.Count; i++)
                    {
                        FtpListingItem lItem = lListingArgs.Listing[i];
                        lSession.ActiveConnection.WriteLine(lItem.ToString());
                    }

                    lSession.ActiveConnection.Close();
                    lSession.ActiveConnection = null;

                    try
                    {
                        e.Connection.WriteLine("226 Transfer complete.");
                    }
                    catch
                    {
                        e.Connection.WriteLine("425 Error while transfering");
                    }
                }

                lSession.RestartPoint = 0;
            }
            else
            {
                e.Connection.WriteLine("503 Bad sequence of commands.");
            }
        }

        public static void Cmd_TYPE(Object sender, CommandEventArgs e)
        {
            FtpSession lSession = (FtpSession)e.Session;

            if (lSession.State == FtpState.LoggedIn)
            {
                if (e.Parameters.Length != 1)
                {
                    e.Connection.WriteLine("501 Syntax error in parameters or arguments.");
                }
                else
                {
                    switch (e.Parameters[0])
                    {
                        case "I":
                            e.Connection.WriteLine("200 Type set to I");
                            lSession.Image = true;
                            break;

                        case "A":
                            e.Connection.WriteLine("200 Type set to A");
                            lSession.Image = false;
                            break;

                        default:
                            e.Connection.WriteLine("501 Syntax error in parameters or arguments.");
                            break;
                    }
                }
            }
            else
            {
                e.Connection.WriteLine("503 Bad sequence of commands.");
            }
        }

        public static void Cmd_REST(Object sender, CommandEventArgs e)
        {
            FtpSession lSession = (FtpSession)e.Session;

            if (lSession.State == FtpState.LoggedIn)
            {
                if (e.Parameters.Length != 1)
                {
                    e.Connection.WriteLine("501 Syntax error in parameters or arguments.");
                }
                else
                {

                    try
                    {
                        Int64 lRestPoint = Int64.Parse(e.Parameters[0]);
                        lSession.RestartPoint = lRestPoint;
                        e.Connection.WriteLine(String.Format("350 Restarting at {0}. Send STORE or RETRIEVE to initiate transfer.", lRestPoint));
                    }
                    catch
                    {
                        e.Connection.WriteLine("501 Syntax error in parameters or arguments.");
                    }
                }
            }
            else
            {
                e.Connection.WriteLine("503 Bad sequence of commands.");
            }
        }

        public static void Cmd_PORT(Object sender, CommandEventArgs e)
        {
            FtpSession lSession = (FtpSession)e.Session;

            if (lSession.State == FtpState.LoggedIn)
            {
                if (e.Parameters.Length != 1)
                {
                    e.Connection.WriteLine("501 Syntax error in parameters or arguments.");
                }
                else
                {
                    String[] lNewPort = e.Parameters[0].Split(new Char[] { ',' });

                    if (lNewPort.Length != 6)
                    {
                        e.Connection.WriteLine("501 Syntax error in parameters or arguments.");
                    }
                    else
                    {
                        IPAddress lNewIp;
                        Int32 lNewPortw;
                        try
                        {
                            lNewIp = IPAddress.Parse(String.Format("{0}.{1}.{2}.{3}", lNewPort[0], lNewPort[1], lNewPort[2], lNewPort[3]));
                            lNewPortw = Byte.Parse(lNewPort[4]) << 8 | Byte.Parse(lNewPort[5]);

                            lSession.Passive = false;
                            try
                            {
                                lSession.ActiveConnection = Client.Connect(lNewIp, lNewPortw, new Binding());
                                e.Connection.WriteLine("200 PORT command succesful");
                            }
                            catch
                            {
                                e.Connection.WriteLine("500 Illegal PORT command ");
                            }
                        }
                        catch
                        {
                            e.Connection.WriteLine("501 Syntax error in parameters or arguments.");
                        }
                    }
                }
            }
            else
            {
                e.Connection.WriteLine("503 Bad sequence of commands.");
            }
        }

        public static void Cmd_QUIT(Object sender, CommandEventArgs e)
        {
            e.Connection.WriteLine(((FtpServer)e.Server).QuitReply);
            e.Connection.Close();
        }

        public static void Cmd_PWD(Object sender, CommandEventArgs e)
        {
            FtpSession lSession = (FtpSession)e.Session;

            if (lSession.State == FtpState.LoggedIn)
            {
                if (e.Parameters.Length != 0)
                    e.Connection.WriteLine("501 Syntax error in parameters or arguments.");
                else
                    e.Connection.WriteLine(String.Format("257 \"{0}\" is current directory.", lSession.Directory));
            }
            else
            {
                e.Connection.WriteLine("503 Bad sequence of commands.");
            }
        }

        public static void Cmd_CWD(Object sender, CommandEventArgs e)
        {
            FtpSession lSession = (FtpSession)e.Session;

            if (lSession.State == FtpState.LoggedIn)
            {
                if (e.Parameters.Length == 0)
                {
                    e.Connection.WriteLine("501 Syntax error in parameters or arguments.");
                }
                else
                {
                    String lNewDir = e.AllParameters;
                    if (lNewDir.StartsWith("/"))
                    {
                        lNewDir = ValidateDirectory(lNewDir);
                    }
                    else
                    {
                        String lTemp = lSession.Directory;
                        if (!lTemp.EndsWith("/"))
                            lTemp = lTemp + "/";
                        lNewDir = ValidateDirectory(lTemp + lNewDir);
                    }

                    FtpChangeDirectoryArgs lArgs = new FtpChangeDirectoryArgs(e.Session, e.Connection, e.Server);
                    lArgs.NewDirectory = lNewDir;

                    try
                    {
                        ((FtpServer)e.Server).InvokeOnChangeDirectory(lArgs);
                    }
                    catch (FtpException ex)
                    {
                        e.Connection.WriteLine(ex.ToString());
                        return;
                    }
                    catch
                    {
                        e.Connection.WriteLine("500 Internal Error");
                        return;
                    }

                    if (lArgs.ChangeDirOk)
                    {
                        e.Connection.WriteLine("250 CWD command successful, new folder is '{0}'.", lSession.Directory);
                        lSession.Directory = lNewDir;
                    }
                    else
                    {
                        e.Connection.WriteLine("550 Permission Denied.");
                    }
                }
            }
            else
            {
                e.Connection.WriteLine("503 Bad sequence of commands.");
            }
        }

        public static void Cmd_CDUP(Object sender, CommandEventArgs e)
        {
            FtpSession lSession = (FtpSession)e.Session;

            if (lSession.State == FtpState.LoggedIn)
            {
                if (e.Parameters.Length != 0)
                {
                    e.Connection.WriteLine("501 Syntax error in parameters or arguments.");
                }
                else
                {
                    String lNewDir = lSession.Directory;
                    if (lNewDir.Length > 2)
                    {
                        Int32 lNewLen = lNewDir.LastIndexOf('/', lNewDir.Length - 2);
                        if (lNewLen <= 0)
                            lNewLen = 1;
                        lNewDir = lNewDir.Substring(0, lNewLen);
                        if (lNewDir.Length == 0)
                            lNewDir = "/";
                    }

                    FtpChangeDirectoryArgs lEventArgs = new FtpChangeDirectoryArgs(e.Session, e.Connection, e.Server);
                    lEventArgs.NewDirectory = lNewDir;
                    try
                    {
                        ((FtpServer)e.Server).InvokeOnChangeDirectory(lEventArgs);
                    }
                    catch (FtpException ex)
                    {
                        e.Connection.WriteLine(ex.ToString());
                        return;
                    }
                    catch
                    {
                        e.Connection.WriteLine("500 Internal Error");
                        return;
                    }

                    if (lEventArgs.ChangeDirOk)
                    {
                        e.Connection.WriteLine("250 CDUP command successful.");
                        lSession.Directory = lNewDir;
                    }
                    else
                    {
                        e.Connection.WriteLine("550 Permission Denied.");
                    }
                }
            }
            else
            {
                e.Connection.WriteLine("503 Bad sequence of commands.");
            }
        }

        public static void Cmd_SYST(Object sender, CommandEventArgs e)
        {
            FtpSession lSession = (FtpSession)e.Session;

            if (lSession.State == FtpState.LoggedIn)
                e.Connection.WriteLine("215 " + ((FtpServer)e.Server).SystemType);
            else
                e.Connection.WriteLine("503 Bad sequence of commands.");
        }

        public static void Cmd_USER(Object sender, CommandEventArgs e)
        {
            FtpSession lSession = (FtpSession)e.Session;

            if (lSession.State == FtpState.Start || lSession.State == FtpState.PasswordRequired)
            {
                if (e.Parameters.Length != 1)
                {
                    e.Connection.WriteLine("501 Syntax error in parameters or arguments.");
                }
                else
                {
                    lSession.Username = e.Parameters[0];
                    e.Connection.WriteLine("331 User name okay, need password.");
                    lSession.State = FtpState.PasswordRequired;
                }
            }
            else
            {
                e.Connection.WriteLine("503 Bad sequence of commands.");
            }
        }

        public static void Cmd_ACCT(Object sender, CommandEventArgs e)
        {
            FtpSession lSession = (FtpSession)e.Session;

            if (lSession.State == FtpState.AccountRequired)
            {
                if (e.Parameters.Length != 1)
                {
                    e.Connection.WriteLine("501 Syntax error in parameters or arguments.");
                }
                else
                {
                    FtpUserAccountArgs lEventArgs = new FtpUserAccountArgs(e.Session, e.Connection, e.Server);
                    lEventArgs.AccountName = e.Parameters[0];
                    try
                    {
                        ((FtpServer)e.Server).InvokeOnAccount(lEventArgs);
                    }
                    catch (FtpException ex)
                    {
                        e.Connection.WriteLine(ex.ToString());
                        return;
                    }
                    catch
                    {
                        e.Connection.WriteLine("500 Internal Error");
                        return;
                    }

                    if (lEventArgs.LoginOk)
                    {
                        e.Connection.WriteLine("230 User logged in, proceed.");
                        lSession.State = FtpState.LoggedIn;
                    }
                    else
                    {
                        lSession.State = FtpState.Start;
                        e.Connection.WriteLine("530 Unable to login");
                    }
                }
            }
            else
            {
                e.Connection.WriteLine("503 Bad sequence of commands.");
            }
        }

        public static void Cmd_PASS(Object sender, CommandEventArgs e)
        {
            FtpSession lSession = (FtpSession)e.Session;

            if (lSession.State == FtpState.PasswordRequired)
            {
                if (e.Parameters.Length != 1)
                {
                    e.Connection.WriteLine("501 Syntax error in parameters or arguments.");
                }
                else
                {
                    FtpUserLoginEventArgs lEventArgs = new FtpUserLoginEventArgs(e.Session, e.Connection, e.Server);
                    lEventArgs.UserName = lSession.Username;
                    lEventArgs.Password = e.Parameters[0];
                    try
                    {
                        ((FtpServer)e.Server).InvokeOnUserLogin(lEventArgs);
                    }
                    catch (FtpException ex)
                    {
                        e.Connection.WriteLine(ex.ToString());
                        return;
                    }
                    catch
                    {
                        e.Connection.WriteLine("500 Internal Error");
                        return;
                    }

                    if (lEventArgs.LoginOk)
                    {
                        if (lEventArgs.NeedAccount)
                        {
                            lSession.State = FtpState.AccountRequired;
                            e.Connection.WriteLine("332 Need account for login.");
                        }
                        else
                        {
                            lSession.State = FtpState.LoggedIn;
                            e.Connection.WriteLine("230 User logged in, proceed.");
                        }
                    }
                    else
                    {
                        lSession.State = FtpState.Start;
                        e.Connection.WriteLine("530 Unable to login");
                    }
                }
            }
            else
            {
                e.Connection.WriteLine("503 Bad sequence of commands.");
            }
        }

        internal protected override void HandleCommandException(Connection connection, Exception exception)
        {
            if (this.Detailed500Errors)
            {
                connection.WriteLine(String.Format("500 Internal Error: ({0}) {1}", exception.GetType().FullName, CleanStringForCommandResponse(exception.Message)));
            }
            else
            {
                connection.WriteLine("500 Internal Error");
            }

            // Don't call base, as we wanna keep the connection open.
        }
    }

    public enum FtpState
    {
        Start, // Connection is opened
        PasswordRequired, // USER command was given
        AccountRequired, // PASS command was ok, server requires ACCOUNT
        LoggedIn,
        Transfer
    };

    public class FtpSession : CommandBasedSession
    {
        public FtpSession()
        {
            this.Directory = "/";
            this.RenameFrom = null;
            this.UserData = null;
            this.Image = false;
            this.State = FtpState.Start;
            this.RestartPoint = 0;
            this.PassiveServer = null;
            this.ActiveConnection = null;
            this.TransferThread = null;
            this.Passive = false;
            this.Username = null;
        }

        #region Properties
        public virtual String Directory
        {
            get
            {
                return this.fDirectory;
            }
            set
            {
                this.fDirectory = value;
            }
        }
        private String fDirectory;

        public virtual String RenameFrom
        {
            get
            {
                return this.fRenameFrom;
            }
            set
            {
                this.fRenameFrom = value;
            }
        }
        private String fRenameFrom;

        public Object UserData
        {
            get
            {
                return this.fUserData;
            }
            set
            {
                this.fUserData = value;
            }
        }
        private Object fUserData;

        public Boolean Image
        {
            get
            {
                return this.fImage;
            }
            set
            {
                this.fImage = value;
            }
        }
        private Boolean fImage;

        public FtpState State
        {
            get
            {
                return this.fState;
            }
            set
            {
                this.fState = value;
            }
        }
        private FtpState fState;

        public Int64 RestartPoint
        {
            get
            {
                return this.fRestartPoint;
            }
            set
            {
                this.fRestartPoint = value;
            }
        }
        private Int64 fRestartPoint;

        public SimpleServer PassiveServer
        {
            get
            {
                return this.fPassiveServer;
            }
            set
            {
                this.fPassiveServer = value;
            }
        }
        private SimpleServer fPassiveServer;

        public Connection ActiveConnection
        {
            get
            {
                return this.fActiveConnection;
            }
            set
            {
                this.fActiveConnection = value;
            }
        }
        private Connection fActiveConnection;

        internal FtpTransferThread TransferThread
        {
            get
            {
                return this.fTransferThread;
            }
            set
            {
                this.fTransferThread = value;
            }
        }
        private FtpTransferThread fTransferThread;

        public Boolean Passive
        {
            get
            {
                return this.fPassive;
            }
            set
            {
                this.fPassive = value;

                if (!this.fPassive && this.fPassiveServer != null)
                {
                    this.fPassiveServer.Close();
                    this.fPassiveServer = null;
                }
            }
        }
        private Boolean fPassive;

        public String Username
        {
            get
            {
                return this.fUsername;
            }
            set
            {
                this.fUsername = value;
            }
        }
        private String fUsername;
        #endregion
    }

    #region 'FtpException' custom exception class

    [Serializable]
    public class FtpException : System.ApplicationException
    {
        public FtpException()
            : base()
        {
            this.fCode = 500;
        }

        public FtpException(String message)
            : base(message)
        {
            this.fCode = 500;
        }

        public FtpException(String message, Exception e)
            : base(message, e)
        {
            this.fCode = 500;
        }

        public FtpException(Int32 code, String message)
            : this(message)
        {
            this.fCode = code;
        }

        public FtpException(String message, Int32 code)
            : this(message)
        {
            this.fCode = code;
        }

        public FtpException(String message, Int32 code, Exception e)
            : this(message, e)
        {
            this.fCode = code;
        }

        public Int32 Code
        {
            get
            {
                return this.fCode;
            }
        }
        private Int32 fCode;

        public override String ToString()
        {
            return String.Format("{0} {1}", this.fCode, this.Message);
        }
    }
    #endregion

    #region Transfer Thread
    class FtpTransferThread
    {
        private FtpTransferEventArgs fEventArgs;
        private Boolean fStore;

        public FtpTransferThread(FtpTransferEventArgs e, Boolean store)
        {
            this.fEventArgs = e;
            this.fStore = store;

            new Thread(new ThreadStart(Execute)).Start();
        }

        public void Abort()
        {
            try
            {
                ((FtpSession)this.fEventArgs.Session).ActiveConnection.Close();
            }
            catch
            {
            }
        }

        public static String GetFirstLine(String value)
        {
            if (value.IndexOf("\r\n") != -1)
            {
                value = value.Substring(0, value.IndexOf("\r\n"));
            }

            if (value.IndexOf("\n") != -1)
            {
                value = value.Substring(0, value.IndexOf("\n"));
            }

            return value;
        }

        private void Execute()
        {
            Boolean lOk = true;

            try
            {
                if (fStore)
                {
                    this.fEventArgs.DataChannel = ((FtpSession)this.fEventArgs.Session).ActiveConnection;
                    ((FtpServer)this.fEventArgs.Server).InvokeOnStoreFile(this.fEventArgs);
                }
                else
                {
                    this.fEventArgs.DataChannel = ((FtpSession)this.fEventArgs.Session).ActiveConnection;
                    ((FtpServer)this.fEventArgs.Server).InvokeOnRetrieveFile(this.fEventArgs);
                }

            }
            catch (FtpException ex)
            {
                this.fEventArgs.Connection.WriteLine(ex.ToString());
                lOk = false;
            }
            catch (Exception ex)
            {
                this.fEventArgs.Connection.WriteLine("500 Internal Error " + FtpTransferThread.GetFirstLine(ex.Message));
                lOk = false;
            }

            ((FtpSession)this.fEventArgs.Session).State = FtpState.LoggedIn;
            try
            {
                ((FtpSession)this.fEventArgs.Session).ActiveConnection.Close();
            }
            catch
            {
            }

            ((FtpSession)this.fEventArgs.Session).ActiveConnection = null;

            if (lOk)
                this.fEventArgs.Connection.WriteLine("226 Transfer complete");

            ((FtpSession)this.fEventArgs.Session).TransferThread = null;
        }
    }
    #endregion
}