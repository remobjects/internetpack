/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2015. All rights reserved.
---------------------------------------------------------------------------*/

using System;
using System.Security;

namespace RemObjects.InternetPack
{
	public class SslNeedPasswordEventArgs : EventArgs
	{
		public SecureString Password
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
		private SecureString fPassword;

		public String PasswordString
		{
			get
			{
				return null;
			}
			set
			{
				if (value == null)
				{
					this.fPassword = null;
					return;
				}

				this.fPassword = new SecureString();
				for (Int32 i = 0; i < value.Length; i++)
				{
					this.fPassword.AppendChar(value[i]);
				}
			}
		}
	}
}