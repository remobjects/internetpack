/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET - Core Library
  (c)opyright RemObjects Software, LLC. 2003-2014. All rights reserved.

  Using this code requires a valid license of the RemObjects Internet Pack
  which can be obtained at http://www.remobjects.com?ip.
---------------------------------------------------------------------------*/

using System;

namespace RemObjects.InternetPack.Ftp
{
	[Serializable]
	public class FtpException : Exception
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
		private readonly Int32 fCode;

		public override String ToString()
		{
			return String.Format("{0} {1}", this.fCode, this.Message);
		}
	}
}