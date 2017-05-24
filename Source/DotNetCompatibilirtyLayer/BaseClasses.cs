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

	public class ArgumentOutOfRangeException : RTLException {
		public ArgumentOutOfRangeException() : base("Argument out of range") {}
	}
	public class ObjectDisposedException : Exception {}

    #if macos || ios || cooper
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

    #if cooper
    public interface Attribute
    {
    
    }
    #endif

	#endif

	#if ECHOES
	public class Monitor : Object {}
	#endif
}