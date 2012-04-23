/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET - Core Library
  (c)opyright RemObjects Software, LLC. 2003-2012. All rights reserved.

  Using this code requires a valid license of the RemObjects Internet Pack
  which can be obtained at http://www.remobjects.com?ip.
---------------------------------------------------------------------------*/

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;

namespace RemObjects.InternetPack.Messages
{
    public class HeaderField : Hashtable
    {
        public HeaderField(String value)
        {
            this.fUnnamedValue = String.Empty;
            this.Value = value;
        }

        #region Private fields
        private String fUnnamedValue;
        #endregion

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
            String lPropertyValue;
            Int32 lEquals;
            Int32 lSemi;

            while (lRemainder.Length > 0)
            {
                if (lRemainder.IndexOf('=') >= 0)
                {
                    if (lRemainder[0] == '"')
                    {
                        lRemainder = lRemainder.Substring(1);
                        lSemi = lRemainder.IndexOf("\";");
                        if (lSemi >= 0)
                        {
                            lPropertyValue = lRemainder.Substring(0, lSemi - 1);
                            lRemainder = lRemainder.Substring(lSemi + 1);
                        }
                        else
                        {
                            lPropertyValue = lRemainder.Substring(0, lRemainder.Length - 1);
                            lRemainder = "";
                        }
                    }
                    else
                    {
                        lSemi = lRemainder.IndexOf(';');
                        if (lSemi >= 0)
                        {
                            lPropertyValue = lRemainder.Substring(0, lSemi);
                            lRemainder = lRemainder.Substring(lSemi + 1);
                        }
                        else
                        {
                            if ((lRemainder[0] == '"') && (lRemainder[lRemainder.Length - 1] == '"'))
                                lPropertyValue = lRemainder.Substring(1, lRemainder.Length - 2);
                            else
                                lPropertyValue = lRemainder;
                            lRemainder = "";
                        }
                    }
                }
                else
                {
                    if ((lRemainder[0] == '"') && (lRemainder[lRemainder.Length - 1] == '"'))
                        lPropertyValue = lRemainder.Substring(1, lRemainder.Length - 2);
                    else
                        lPropertyValue = lRemainder;
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

                lEquals = lRemainder.IndexOf('=');
                if (lEquals >= 0)
                {
                    lProperty = lRemainder.Substring(0, lEquals);
                    lRemainder = lRemainder.Substring(lEquals + 1);
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
            : base()
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