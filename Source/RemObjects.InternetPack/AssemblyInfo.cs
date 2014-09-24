/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET - Core Library
  (c)opyright RemObjects Software, LLC 2003-2014. All rights reserved.

  Using this code requires a valid license of the RemObjects Internet Pack
  which can be obtained at http://www.remobjects.com?ip.
---------------------------------------------------------------------------*/

using System;
using System.Reflection;
using System.Resources;

[assembly: AssemblyTitle("")]
[assembly: AssemblyDescription("RemObjects Internet Pack for .NET - Core Library")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("RemObjects Software, LLC")]
[assembly: AssemblyProduct("RemObjects Internet Pack for .NET")]
[assembly: AssemblyCopyright("Copyright RemObjects Software, LLC 2003-2013. All Rights Reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyVersion("2.0.0.0")]

[assembly: NeutralResourcesLanguage("")]
[assembly: CLSCompliant(true)]

#if REMOBJECTS_SIGN_ASSEMBLY && !COMPACTFRAMEWORK
[assembly: AssemblyKeyName("RemObjectsSoftware")]
[assembly: System.Security.AllowPartiallyTrustedCallers()]
#endif