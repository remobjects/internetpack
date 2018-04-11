/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

namespace RemObjects.InternetPack
{
	public class SslNeedPasswordEventArgs : EventArgs
	{
		public SecureString Password { get; set; }

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
					this.Password = null;
					return;
				}

				this.Password = new SecureString();
				for (Int32 i = 0; i < value.Length; i++)
				{
					this.Password.AppendChar(value[i]);
				}
			}
		}
	}
}