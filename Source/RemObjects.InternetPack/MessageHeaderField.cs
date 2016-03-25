/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;

namespace RemObjects.InternetPack.Messages
{
	public class HeaderField : Hashtable
	{
		#region Private fields
		private String fUnnamedValue;
		#endregion

		public HeaderField(String value)
		{
			this.fUnnamedValue = String.Empty;
			this.Value = value;
		}

		public String Value
		{
			get
			{
				return this.fValue;
			}
			set
			{
				this.fValue = value;
				this.Parse();
			}
		}
		private String fValue;

		public void Parse()
		{
			//String lValue = "";
			String lRemainder = fValue;
			String lProperty = null;

			while (lRemainder.Length > 0)
			{
				String lPropertyValue;
				if (lRemainder.IndexOf('=') >= 0)
				{
					Int32 lSemicolonPosition;
					if (lRemainder[0] == '"')
					{
						lRemainder = lRemainder.Substring(1);
						lSemicolonPosition = lRemainder.IndexOf("\";", StringComparison.Ordinal);
						if (lSemicolonPosition >= 0)
						{
							lPropertyValue = lRemainder.Substring(0, lSemicolonPosition - 1);
							lRemainder = lRemainder.Substring(lSemicolonPosition + 1);
						}
						else
						{
							lPropertyValue = lRemainder.Substring(0, lRemainder.Length - 1);
							lRemainder = "";
						}
					}
					else
					{
						lSemicolonPosition = lRemainder.IndexOf(';');
						if (lSemicolonPosition >= 0)
						{
							lPropertyValue = lRemainder.Substring(0, lSemicolonPosition);
							lRemainder = lRemainder.Substring(lSemicolonPosition + 1);
						}
						else
						{
							lPropertyValue = (lRemainder[0] == '"') && (lRemainder[lRemainder.Length - 1] == '"')
												? lRemainder.Substring(1, lRemainder.Length - 2)
												: lRemainder;
							lRemainder = "";
						}
					}
				}
				else
				{
					lPropertyValue = (lRemainder[0] == '"') && (lRemainder[lRemainder.Length - 1] == '"')
										? lRemainder.Substring(1, lRemainder.Length - 2)
										: lRemainder;
					lRemainder = "";
				}

				lPropertyValue = lPropertyValue.Trim();

				if (String.IsNullOrEmpty(lProperty))
					this.fUnnamedValue = lPropertyValue;
				else
				{
					if (this.ContainsKey(lProperty))
						this[lProperty] = this[lProperty] + lPropertyValue;
					else
						this.Add(lProperty, lPropertyValue);
				}

				lRemainder = lRemainder.Trim();

				Int32 lEqualsPosition = lRemainder.IndexOf('=');
				if (lEqualsPosition >= 0)
				{
					lProperty = lRemainder.Substring(0, lEqualsPosition);
					lRemainder = lRemainder.Substring(lEqualsPosition + 1);
				}
			}
		}

		public override String ToString()
		{
			StringBuilder lResult = new StringBuilder("");
			Boolean lFirst = true;

			foreach (String key in Keys)
			{
				if (lFirst)
					lFirst = false;
				else
					lResult.Append("; ");

				lResult.AppendFormat(System.Globalization.CultureInfo.InvariantCulture, "{0}={1}", key, this[key]);
			}

			if (fUnnamedValue != "")
			{
				if (!lFirst)
					lResult.Append("; ");

				lResult.Append(fUnnamedValue);
			}

			return lResult.ToString();
		}
	}

	public class HeaderFields : NameObjectCollectionBase
	{
		public HeaderFields()
		{
		}

		public void Add(String name, HeaderField field)
		{
			BaseAdd(name, field);
		}

		public void Remove(String name)
		{
			BaseRemove(name);
		}

		public void Clear()
		{
			BaseClear();
		}

		public HeaderField this[Int32 index]
		{
			get
			{
				return BaseGet(index) as HeaderField;
			}
			set
			{
				BaseSet(index, value);
			}
		}

		public HeaderField this[String index]
		{
			get
			{
				HeaderField lResult = BaseGet(index) as HeaderField;
				if (lResult == null)
				{
					lResult = new HeaderField("");
					BaseSet(index, lResult);
				}
				return lResult;
			}
			set
			{
				BaseSet(index, value);
			}
		}
	}
}