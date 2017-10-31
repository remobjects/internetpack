/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

using RemObjects.InternetPack.Shared.Base;

namespace RemObjects.InternetPack.Ldap
{
	// http://tools.ietf.org/html/rfc4511 << ldap
	// http://tools.ietf.org/html/rfc3641 << asn1 (encoding of values)

	public class LdapException : Exception
	{
		public LdapException(String message)
		: base(message)
		{
		}

		public LdapException(String message, Int32 code)
		: base(message)
		{
			this.fCode = code;
		}

		public LdapException(Int32 code)
		: base(ErrorToString(code))
		{
			this.fCode = code;
		}

		public Int32 Code
		{
			get
			{
				return fCode;
			}
		}
		private readonly Int32 fCode;

		public static String ErrorToString(Int32 code)
		{
			switch (code)
			{
				case 0: return "Success";
				case 1: return "Operations Error";
				case 2: return "Protocol Error";
				case 3: return "Timelimit Exceeded";
				case 4: return "Sizelimit Exceeded";
				case 5: return "Compare False";
				case 6: return "Compare True";
				case 7: return "Authentication Method Not Supported";
				case 8: return "Strong Authentication Required";
				case 9: return "Partial Results";
				case 10: return "Referral";
				case 11: return "Administrative Limit Exceeded";
				case 12: return "Unavailable Critical Extension";
				case 13: return "Confidentiality Required";
				case 14: return "SASL Bind In Progress";
				case 16: return "No Such Attribute";
				case 17: return "Undefined Attribute Type";
				case 18: return "Inappropriate Matching";
				case 19: return "Constraint Violation";
				case 20: return "Attribute Or Value Exists";
				case 21: return "Invalid Attribute Syntax";
				case 32: return "No Such Object";
				case 33: return "Alias Problem";
				case 34: return "Invalid DN Syntax";
				case 35: return "Is Leaf";
				case 36: return "Alias Dereferencing Problem";
				case 48: return "Inappropriate Authentication";
				case 49: return "Invalid Credentials";
				case 50: return "Insufficient Access Rights";
				case 51: return "Busy";
				case 52: return "Unavailable";
				case 53: return "Unwilling To Perform";
				case 54: return "Loop Detect";
				case 64: return "Naming Violation";
				case 65: return "Object Class Violation";
				case 66: return "Not Allowed On Non-leaf";
				case 67: return "Not Allowed On RDN";
				case 68: return "Entry Already Exists";
				case 69: return "Object Class Modifications Prohibited";
				case 71: return "Affects Multiple DSAs";
				case 80: return "Other";
				case 81: return "Server Down";
				case 82: return "Local Error";
				case 83: return "Encoding Error";
				case 84: return "Decoding Error";
				case 85: return "Ldap Timeout";
				case 86: return "Authentication Unknown";
				case 87: return "Filter Error";
				case 88: return "User Cancelled";
				case 89: return "Parameter Error";
				case 90: return "No Memory";
				case 91: return "Connect Error";
				case 92: return "Ldap Not Supported";
				case 93: return "Control Not Found";
				case 94: return "No Results Returned";
				case 95: return "More Results To Return";
				case 96: return "Client Loop";
				case 97: return "Referral Limit Exceeded";
				case 112: return "TLS not supported";
				case 113: return "SSL handshake failed";
				case 114: return "SSL Provider not found";
				default: return "Unknown Error";
			}
		}
	}
}