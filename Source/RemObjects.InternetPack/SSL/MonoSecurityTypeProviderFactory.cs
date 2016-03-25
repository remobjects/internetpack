/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

using System;

namespace RemObjects.InternetPack
{
	static class MonoSecurityTypeProviderFactory
	{
		private static readonly Object fLock = new Object();
		private static volatile IMonoSecurityTypeProvider fProvider;

		public static IMonoSecurityTypeProvider CreateTypeProvider()
		{
			// Double-lock implementation (to avoid excessive locking)
			if (fProvider != null)
			{
				return fProvider;
			}

			lock (fLock)
			{
				// Check again
				if (fProvider != null)
				{
					return fProvider;
				}

				try
				{
					// Constructor can throw if Mono.Security cannot be loaded for some reason
					IMonoSecurityTypeProvider lTypeProvider = new MonoSecurityTypeProvider();

					fProvider = lTypeProvider;

					return lTypeProvider;
				}
				catch (Exception)
				{
					return null;
				}
			}
		}
	}
}