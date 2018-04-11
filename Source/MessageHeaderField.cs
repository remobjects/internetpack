/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

namespace RemObjects.InternetPack.Messages
{
	public class HeaderField
	{
		#region Private fields
		private String fUnnamedValue;
		#endregion

		private Dictionary<String, String> fData = new Dictionary<String, String>();

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
						lSemicolonPosition = lRemainder.IndexOf("\";");
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
					if (this.fData.ContainsKey(lProperty))
						this.fData[lProperty] = this.fData[lProperty] + lPropertyValue;
					else
						this.fData.Add(lProperty, lPropertyValue);
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

			foreach (String key in fData.Keys)
			{
				if (lFirst)
					lFirst = false;
				else
					lResult.Append("; ");

				lResult.AppendFormat(ToString(), "{0}={1}", key, this.fData[key]);
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

	public class HeaderFields
	{
		private List<KeyValuePair<String, HeaderField>> fData = new List<KeyValuePair<String, HeaderField>>();

		public HeaderFields()
		{
		}

		private int IndexOf(String aName)
		{
			for(var i = 0; i < fData.Count; i++)
				if (aName.Equals(fData[i].Key))
					return i;

			return -1;
		}

		public void Add(String name, HeaderField field)
		{
			fData.Add(new KeyValuePair<String, HeaderField>(name, field));
		}

		public void Remove(String name)
		{
			var lIndex = IndexOf(name);
			if (lIndex >= 0)
				fData.RemoveAt(lIndex);
		}

		public void Clear()
		{
			fData.RemoveAll();
		}

		public HeaderField this[Int32 index]
		{
			get
			{
				return fData[index].Value;
			}
			set
			{
				fData[index] = new KeyValuePair<String, HeaderField>(fData[index].Key, value);
			}
		}

		public HeaderField this[String index]
		{
			get
			{
				var lIndex = IndexOf(index);
				HeaderField lResult;
				if (lIndex == -1)
				{
					lResult = new HeaderField("");
					Add(index, lResult);
				}
				else
					lResult = fData[lIndex].Value;

				return lResult;
			}
			set
			{
				var lIndex = IndexOf(index);
				if (lIndex == -1)
					fData.Add(new KeyValuePair<String, HeaderField>(index, value));
				else
					fData[lIndex] = new KeyValuePair<String, HeaderField>(index, value);
			}
		}

		public int Count
		{
			get
			{
				return fData.Count;
			}
		}

		public String GetKey(int index)
		{
			return fData[index].Key;
		}
	}
}