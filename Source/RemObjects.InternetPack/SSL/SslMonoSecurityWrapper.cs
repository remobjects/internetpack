/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

using System;
using System.Reflection;

namespace RemObjects.InternetPack
{
	static class SslMonoSecurityWrapper
	{
		private static readonly Object fLock = new Object();
		private static volatile Assembly fAssembly;

		public static Assembly GetAssembly()
		{
			// Double-lock implementation (to avoid excessive locking)
			if (fAssembly != null)
			{
				return fAssembly;
			}

			lock (fLock)
			{
				// Check again
				if (fAssembly != null)
				{
					return fAssembly;
				}

				try
				{
					Assembly lAssembly = null;
					// Try to load .NET 4.0+ version first
					if (Environment.Version.Major >= 4)
					{
						try
						{
							lAssembly = Assembly.Load("Mono.Security, Version=4.0.0.0, Culture=neutral, PublicKeyToken=0738eb9f132ed756");
						}
						catch (Exception)
						{
							//
						}
					}

					// If we failed for any reason - try to load .NET 2.0 version
					if (lAssembly == null)
					{
						lAssembly = Assembly.Load("Mono.Security, Version=2.0.0.0, Culture=neutral, PublicKeyToken=0738eb9f132ed756");
					}

					fAssembly = lAssembly;

					return lAssembly;
				}
				catch (Exception)
				{
					return null;
				}
			}
		}
	}
}