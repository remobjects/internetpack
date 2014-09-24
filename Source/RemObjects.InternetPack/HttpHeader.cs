/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET - Core Library
  (c)opyright RemObjects Software, LLC. 2003-2014. All rights reserved.

  Using this code requires a valid license of the RemObjects Internet Pack
  which can be obtained at http://www.remobjects.com?ip.
---------------------------------------------------------------------------*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace RemObjects.InternetPack.Http
{
	public class HttpHeader
	{
		#region Private fields
		List<String> fValues = new List<String>();
		#endregion

		public HttpHeader(String name, String value)
		{
			this.Name = name;
			this.fValues.Add(value);
		}

		public HttpHeader(String line)
		{
			Int32 lPos = line.IndexOf(":");
			if (lPos == -1)
				throw new HttpHeaderException("Invalid HTTP Header Line \"" + line + "\"");

			this.Name = line.Substring(0, lPos);
			this.fValues.Add(line.Substring(lPos + 2));
		}

		#region ToString
		public override String ToString()
		{
			if (this.Count == 1)
				return String.Format("{0}: {1}", this.Name, this.fValues[0]);

			StringBuilder lResult = new StringBuilder();
			for (Int32 i = 0; i < this.Count; i++)
			{
				if (i > 0)
					lResult.Append("\r\n");

				lResult.Append(String.Format("{0}: {1}", this.Name, this.fValues[i]));
			}

			return lResult.ToString();
		}
		#endregion

		#region Properties
		public String Name
		{
			get
			{
				return this.fName;
			}
			set
			{
				this.fName = value;
			}
		}
		private String fName;

		public Int32 Count
		{
			get
			{
				return this.fValues.Count;
			}
		}

		public String Value
		{
			get
			{
				if (this.Count == 1)
					return this.fValues[0];

				StringBuilder lResult = new StringBuilder();
				for (Int32 i = 0; i < this.Count; i++)
				{
					if (i > 0)
						lResult.Append(",");

					lResult.Append(String.Format("\"{0}\"", this.fValues[i]));
				}

				return lResult.ToString();
			}
			set
			{
				this.fValues.Clear();
				this.fValues.Add(value);
			}
		}
		#endregion

		public String Get(Int32 index)
		{
			return this.fValues[index].ToString();
		}

		public void Add(String item)
		{
			this.fValues.Add(item);
		}
	}

	class HttpHeaderEnumerator : IEnumerator
	{
		public HttpHeaderEnumerator(IEnumerator parent)
		{
			this.fParent = parent;
		}

		private readonly IEnumerator fParent;

		#region IEnumerator Members
		public void Reset()
		{
			this.fParent.Reset();
		}

		public object Current
		{
			get
			{
				return ((DictionaryEntry)(this.fParent.Current)).Value;
			}
		}

		public Boolean MoveNext()
		{
			return this.fParent.MoveNext();
		}
		#endregion
	}

	public class HttpHeaders : IEnumerable
	{
		public HttpHeaders()
		{
			this.fHeaders = new Hashtable(StringComparer.OrdinalIgnoreCase);
			this.HttpCode = HttpStatusCode.OK;
			this.Initialize();
		}

		// Cannot use Dictionary<> here because non-generic Enumerator is exposed
		private Hashtable fHeaders;

		public static HttpHeaders Create(Connection connection)
		{
			HttpHeaders lResult = new HttpHeaders();
			if (!lResult.ReadHeader(connection))
				return null;

			return lResult;
		}

		protected virtual void Initialize()
		{
			this.MaxHeaderLines = 100;
			this.MaxHeaderLinesEnabled = true;
		}

		#region Private Helper Methods
		public void ParseFirstLine()
		{
			if (this.fFirstHeader.Length == 0)
				throw new HttpHeaderException("HTTP Header is empty"); ;

			String lHeaderLine = this.fFirstHeader;
			String[] lRequestHeaderValues = lHeaderLine.Split(' ');

			if (lRequestHeaderValues.Length < 3)
				throw new HttpHeaderException("Invalid HTTP Header Line \"" + lHeaderLine + "\"");

			if (lHeaderLine.StartsWith("HTTP/"))
			{
				// HTTP Response
				try
				{
					this.HttpCode = (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), lRequestHeaderValues[1], true);
				}
				catch (ArgumentException)
				{
					this.HttpCode = HttpStatusCode.OK;
				}
			}
			else
			{
				// HTTP Request
				this.fRequestType = lRequestHeaderValues[0];
				this.fRequestPath = lRequestHeaderValues[1];
				this.fRequestVersion = lRequestHeaderValues[2];

				if (this.fRequestVersion.StartsWith("HTTP/"))
					this.fRequestVersion = fRequestVersion.Substring(5);
			}
		}
		#endregion

		#region Properties
		public String RequestType
		{
			get
			{
				return fRequestType;
			}
			set
			{
				fRequestType = value;
			}
		}
		private String fRequestType;

		public String RequestPath
		{
			get
			{
				return fRequestPath;
			}
			set
			{
				fRequestPath = value;
			}
		}
		private String fRequestPath;

		public String RequestVersion
		{
			get
			{
				return fRequestVersion;
			}
			set
			{
				fRequestVersion = value;
			}
		}
		private String fRequestVersion;

		[Obsolete("Access HTTP code using the HttpCode property")]
		public Int32 ResponseCode
		{
			get
			{
				return (Int32)this.HttpCode;
			}
			set
			{
				this.HttpCode = (HttpStatusCode)value;
			}
		}

		public HttpStatusCode HttpCode
		{
			get
			{
				return fHttpCode;
			}
			private set
			{
				this.fHttpCode = value;
			}
		}
		private HttpStatusCode fHttpCode;

		public Int32 MaxHeaderLines
		{
			get
			{
				return fMaxHeaderLines;
			}
			set
			{
				fMaxHeaderLines = value;
			}
		}
		private Int32 fMaxHeaderLines;

		public Boolean MaxHeaderLinesEnabled
		{
			get
			{
				return fMaxHeaderLinesEnabled;
			}
			set
			{
				fMaxHeaderLinesEnabled = value;
			}
		}
		private Boolean fMaxHeaderLinesEnabled;

		public String ContentType
		{
			get
			{
				if (ContainsHeaderValue(CONTENT_TYPE))
					return GetHeaderValue(CONTENT_TYPE);

				return "";
			}
			set
			{
				SetHeaderValue(CONTENT_TYPE, value);
			}
		}
		private const String CONTENT_TYPE = "Content-Type";

		public String FirstHeader
		{
			get
			{
				return fFirstHeader;
			}
			set
			{
				fFirstHeader = value;
			}
		}
		private String fFirstHeader;

		public Int32 Count
		{
			get
			{
				return fHeaders.Count;
			}
		}

		public HttpHeader this[String key]
		{
			get
			{
				if (this.fHeaders.ContainsKey(key))
					return (HttpHeader)this.fHeaders[key];

				return null;
			}
		}
		#endregion

		#region Methods
		// This method returns:
		// 1. NULL if incoming datastream doesn't contain at least 4 bytes
		// 2. Empty String is HTTP method is unknown
		// 3. HTTP method name
		private static String ReadHttpMethodName(Connection connection)
		{
			Byte[] lBuffer = new Byte[4];

			if (connection.Receive(lBuffer, 0, 4) < 4)
				return null;

			String lHttpMethodName = Encoding.ASCII.GetString(lBuffer, 0, 4);

			if ((lHttpMethodName == "POST") || (lHttpMethodName == "GET ") || (lHttpMethodName == "HTTP") ||
					(lHttpMethodName == "HEAD") || (lHttpMethodName == "PUT "))
				return lHttpMethodName;

			if (lHttpMethodName == "MERG")
			{
				connection.Read(lBuffer, 0, 1);
				if (lBuffer[0] != (Byte)'E')
					return String.Empty;

				return "MERGE";
			}

			if (lHttpMethodName == "DELE")
			{
				connection.Read(lBuffer, 0, 2);
				if (lBuffer[0] != (Byte)'T' || lBuffer[1] != (Byte)'E')
					return String.Empty;

				return "DELETE";
			}

			if (lHttpMethodName == "OPTI")
			{
				connection.Read(lBuffer, 0, 3);
				if (lBuffer[0] != (Byte)'O' || lBuffer[1] != (Byte)'N' || lBuffer[2] != (Byte)'S')
					return String.Empty;

				return "OPTIONS";
			}

			return String.Empty;
		}

		public Boolean ReadHeader(Connection connection)
		{
			this.fFirstHeader = String.Empty;

			String lStart = HttpHeaders.ReadHttpMethodName(connection);

			if (lStart == null)
				return false;

			if (String.Equals(lStart, String.Empty, StringComparison.Ordinal))
				throw new HttpRequestInvalidException(HttpStatusCode.InternalServerError, "Invalid HTTP Request, 'POST', 'MERGE', 'GET', 'DELETE', 'PUT' or 'HEAD' header expected.");

			String lHeaderLine;
			do
			{
				lHeaderLine = connection.ReadLine();

				if (!String.IsNullOrEmpty(lHeaderLine))
				{
					if (this.fFirstHeader.Length == 0)
					{
						this.fFirstHeader = lStart + lHeaderLine;
					}
					else
					{
						Int32 lPos = lHeaderLine.IndexOf(":");
						if (lPos == -1)
							throw new HttpHeaderException("Invalid HTTP Header Line \"" + lHeaderLine + "\"");

						String lName = lHeaderLine.Substring(0, lPos);
						String lValue = null;

						// There should be a space after the ":" , but at least one custome said that this is not
						// always true
						// So we check if there is space after the ":"
						if (lHeaderLine.Length > lPos + 1)
						{
							if (lHeaderLine[lPos + 1] == ' ')
								lValue = lHeaderLine.Substring(lPos + 2);
							else
								lValue = lHeaderLine.Substring(lPos + 1);
						}

						HttpHeader lHeader = this[lName];
						if (lHeader == null)
						{
							lHeader = new HttpHeader(lName, lValue);
							fHeaders.Add(lName, lHeader);
						}
						else
						{
							lHeader.Add(lValue);
						}
					}
				}

				if (this.MaxHeaderLinesEnabled && this.fHeaders.Count > this.MaxHeaderLines - 1) // -1 because FirstHeader is not in hashtable
				{
					connection.Disconnect();
					throw new HttpHeaderException(String.Format("Too many header lines received (maximum is set to {0})", MaxHeaderLines));
				}
			}
			while (!String.IsNullOrEmpty(lHeaderLine));

			this.ParseFirstLine();

			return true;
		}

		public void WriteHeader(Connection connection)
		{
			connection.WriteLine(fFirstHeader);

			foreach (HttpHeader header in this)
			{
				connection.WriteLine(header.ToString());
			}
			connection.WriteLine("");
		}

		[Obsolete("Provide HTTP code using a System.Net.HttpStatusCode value")]
		public void SetResponseHeader(String version, Int32 code)
		{
			this.SetResponseHeader(version, (HttpStatusCode)code);
		}

		public void SetResponseHeader(String version, HttpStatusCode code)
		{
			this.fFirstHeader = String.Format("HTTP/{0} {1} {2}", version, ((Int32)code).ToString(), code.ToString());
			this.HttpCode = code;
		}

		public void SetRequestHeader(String version, String requestType, String requestPath)
		{
			fFirstHeader = String.Format("{0} {1} HTTP/{2}", requestType, requestPath, version);
		}

		public Boolean ContainsHeaderValue(String key)
		{
			return this.fHeaders.ContainsKey(key);
		}

		public void SetHeaderValue(String name, String value)
		{
			HttpHeader lHeader = this[name];

			if (lHeader == null)
			{
				lHeader = new HttpHeader(name, value);
				fHeaders[name] = lHeader;
			}
			else
			{
				lHeader.Value = value;
			}
		}

		public String GetHeaderValue(String name)
		{
			HttpHeader lHeader = this[name];

			if (lHeader != null)
				return lHeader.Value;

			return null;
		}

		public override String ToString()
		{
			StringBuilder lResult = new StringBuilder();
			lResult.Append(fFirstHeader);
			lResult.Append("\r\n");

			foreach (HttpHeader header in this)
			{
				lResult.Append(header.ToString());
				lResult.Append("\r\n");
			}

			lResult.Append("\r\n");

			return lResult.ToString();
		}
		#endregion

		#region IEnumerable Members
		public IEnumerator GetEnumerator()
		{
			return new HttpHeaderEnumerator(this.fHeaders.GetEnumerator());
		}
		#endregion
	}
}