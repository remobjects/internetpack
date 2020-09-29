namespace RemObjects.InternetPack
{
	#if !ECHOES
	public class Component : Object {
		protected virtual void Dispose(Boolean disposing)
		{
			//(this as IDisposable)?.Dispose();
		}
	}
	public class MarshalByRefObject : Object {}

	// Generated from /Users/mh/Xcode/DerivedData/Fire-beiaefoboptwvtbxtvecylpnprxy/Build/Products/Debug/Fire.app/Contents/Resources/Mono/lib/mono/2.0/mscorlib.dll
	//[ComVisibleAttribute(true)]
	public class EventArgs : Object
	{
		public static EventArgs Empty { get; } = new EventArgs();
		public EventArgs() {}
	}

	// Generated from /Users/mh/Xcode/DerivedData/Fire-beiaefoboptwvtbxtvecylpnprxy/Build/Products/Debug/Fire.app/Contents/Resources/Mono/lib/mono/2.0/mscorlib.dll
	//[ComVisibleAttribute(true)]
	public delegate void AsyncCallback(IAsyncResult ar);

	// Generated from /Users/mh/Xcode/DerivedData/Fire-beiaefoboptwvtbxtvecylpnprxy/Build/Products/Debug/Fire.app/Contents/Resources/Mono/lib/mono/2.0/mscorlib.dll
	//[ComVisibleAttribute(true)]
	public delegate void EventHandler(Object sender, EventArgs e);

	#if cooper
	public interface SerializableAttribute : java.lang.annotation.Annotation {}
	public interface BrowsableAttribute : java.lang.annotation.Annotation {
		bool Value();
	}
	public interface CategoryAttribute : java.lang.annotation.Annotation {
		string Value();
	}
	public interface DefaultValueAttribute : java.lang.annotation.Annotation {
		object Value();
	}
	#else
	public class SerializableAttribute : Attribute {}
	public class BrowsableAttribute : Attribute {
		public BrowsableAttribute(bool ignore) {}
	}
	public class CategoryAttribute : Attribute {
		public CategoryAttribute(string ignore) {}
	}
	public class DefaultValueAttribute : Attribute {
		public DefaultValueAttribute(object ignore) {}
	}
	#endif

	public class ArgumentOutOfRangeException : RTLException {
		public ArgumentOutOfRangeException() : base("Argument out of range") {}

		public ArgumentOutOfRangeException(string message) : base(message) {}
	}
	public class ObjectDisposedException : Exception {}

	#if toffee || cooper
	public interface IDisposable
	{
		void Dispose();
	}

	public static class Array : Object
	{
		public static void Copy(byte[] aSource, Int32 aSourceOffset, byte[] aDest, Int32 aDestOffset, Int32 aCount)
		{
			for (int i = 0; i < aCount; i++)
				aDest[aDestOffset + i] = aSource[aSourceOffset + i];
		}
	}
	#endif

	#endif

	#if island
	public class Exception : RemObjects.Elements.System.Exception
	{
		public Exception() : base("Exception happened")
		{
		}

		public Exception(String message) : base(message)
		{
		}

		public Exception(String message, Exception innerException)
			: base(message + ' ' + innerException.ToString())
		{
		}
	}
	#endif

	#if toffee
	public class Exception : Foundation.NSException
	{
		public Exception()
		{
			initWithName("Exception") reason("unknown") userInfo(null);
		}

		public Exception(String message)
		{
			initWithName("Exception") reason(message) userInfo(null);
		}

		public Exception(String message, Exception innerException)
		{
			initWithName("Exception") reason(message + ' ' + innerException.ToString()) userInfo(null);
		}
	}
	#endif

	#if !echoes
	public class ArrayList
	{
		protected List<Object> fList;

		public ArrayList()
		{
			fList = new List<Object>();
		}

		public Object this[Int32 index]
		{
			get
			{
				return fList[index];
			}
		}

		public void Add(Object item)
		{
			fList.Add(item);
		}

		public void Insert(Int32 pos, Object item)
		{
			fList.Insert(pos, item);
		}

		public void Clear()
		{
			fList.RemoveAll();
		}

		public Int32 Count
		{
			get
			{
				return fList.Count;
			}
		}
	}

	public class NameValueCollection
	{
		public NameValueCollection(int capacity, NameValueCollection col) { }
		public NameValueCollection(int capacity) { }
		public NameValueCollection(NameValueCollection col) { }
		public NameValueCollection() { }
		protected void InvalidateCachedArrays() { }
		public virtual void Add(String name, String value) { }
		public void Add(NameValueCollection c) { }
		public virtual void Clear() { }
		public void CopyTo(Array dest, int index) { }
		public bool HasKeys() { }
		public virtual String Get(int index) { }
		public virtual String Get(String name) { }
		public virtual String[] GetValues(int index) { }
		public virtual String[] GetValues(String name) { }
		public virtual void Set(String name, String @value) { }
		public virtual void Remove(String name) { }
		public virtual String GetKey(int index) { }
		//public String this[int index] { get; set; }
		public String this[String name] { get { } set { } }
		public virtual String[] AllKeys { get; set; }
		protected ISequence<string> GetKeys() { }
		public ISequence<string> Keys {get { GetKeys(); } }
	}

	#endif
}