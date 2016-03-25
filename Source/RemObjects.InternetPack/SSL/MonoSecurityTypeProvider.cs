/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Reflection;

namespace RemObjects.InternetPack
{
	sealed class MonoSecurityTypeProvider : IMonoSecurityTypeProvider
	{
		#region Private fields
		private readonly Object fLockRoot;
		private readonly Assembly fAssembly;
		// Own custom type caching is used, because I am completely not sure that Mono's Reflection implementation has one
		private readonly IDictionary<String, Type> fTypeCache;
		#endregion

		public MonoSecurityTypeProvider()
		{
			this.fLockRoot = new Object();
			this.fAssembly = MonoSecurityTypeProvider.LoadMonoSecurityAssembly();
			this.fTypeCache = new Dictionary<String, Type>(16);
		}

		private static Assembly LoadMonoSecurityAssembly()
		{
			// Do not even try to load Mono.Security, Version=4.0.0.0
			// In the latest Mono releases (at least in 3.2.8, one default in Ubuntu/Kubuntu)
			// accessing some of the CertificateBuilder properties throw "Invalid IL code"
			return Assembly.Load("Mono.Security, Version=2.0.0.0, Culture=neutral, PublicKeyToken=0738eb9f132ed756");
		}

		public Type GetType(String typeName)
		{
			Type lRequestedType;
			if (this.fTypeCache.TryGetValue(typeName, out lRequestedType))
			{
				return lRequestedType;
			}

			lock (this.fLockRoot)
			{
				// Double-check
				if (this.fTypeCache.TryGetValue(typeName, out lRequestedType))
				{
					return lRequestedType;
				}

				lRequestedType = this.fAssembly.GetType(typeName, true);

				this.fTypeCache.Add(typeName, lRequestedType);
			}

			return lRequestedType;
		}
	}
}