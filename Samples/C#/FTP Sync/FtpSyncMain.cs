using System;
using System.IO;

namespace FtpSync
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class FtpSyncMain
	{

		/// <summary>
			/// The main entry point for the application.
			/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			FtpSyncWorker lWorker = new FtpSyncWorker();

			if (lWorker.CheckArgs(args))
			{
				lWorker.Sync();
			}
		}   
	}
}
