/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET - Core Library
  (c)opyright RemObjects Software, LLC. 2003-2014. All rights reserved.

  Using this code requires a valid license of the RemObjects Internet Pack
  which can be obtained at http://www.remobjects.com?ip.
---------------------------------------------------------------------------*/

using System;

namespace RemObjects.InternetPack
{
	public interface IConnectionTimeouts
	{
		Int32 Timeout { get; set; }
		Boolean TimeoutEnabled { get; set; }
	}
}