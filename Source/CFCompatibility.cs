/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET - Core Library
  (c)opyright RemObjects Software, LLC. 2003-2012. All rights reserved.

  Using this code requires a valid license of the RemObjects Internet Pack
  which can be obtained at http://www.remobjects.com?ip.
---------------------------------------------------------------------------*/

using System;
using System.Collections;

namespace RemObjects.InternetPack
{
    public class StringCollection : ArrayList
    {
        public new string this[Int32 index]
        {
            get
            {
                return (string)base[index];
            }
            set
            {
                base[index] = value;
            }
        }
    }

    class TypeConverterAttribute : Attribute
    {
        public TypeConverterAttribute(Type type)
        {
        }
    }

    class BrowsableAttribute : Attribute
    {
        public BrowsableAttribute(Boolean browsable)
        {
        }
    }

    class CategoryAttribute : Attribute
    {
        public CategoryAttribute(String category)
        {
        }
    }

    class SerializableAttribute : Attribute
    {
        public SerializableAttribute()
        {
        }
    }

    public static class IntHelper
    {
        public static Boolean TryParse(String text, out Int32 value)
        {
            try
            {
                value = Int32.Parse(text);
                return true;
            }
            catch
            {
                value = 0;
                return false;
            }
        }
    }

    public static class LongHelper
    {
        public static Boolean TryParse(String text, out Int64 value)
        {
            try
            {
                value = Int64.Parse(text);
                return true;
            }
            catch
            {
                value = 0;
                return false;
            }
        }
    }
}