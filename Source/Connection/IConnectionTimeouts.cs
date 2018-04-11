/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

namespace RemObjects.InternetPack
{
	public interface IConnectionTimeouts
	{
		Int32 Timeout { get; set; }
		Boolean TimeoutEnabled { get; set; }
	}
}