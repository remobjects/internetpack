namespace RemObjects.InternetPack
{
	#if !ECHOES
	// Generated from /Users/mh/Xcode/DerivedData/Fire-beiaefoboptwvtbxtvecylpnprxy/Build/Products/Debug/Fire.app/Contents/Resources/Mono/lib/mono/2.0/mscorlib.dll
	//[ComVisibleAttribute(true)]
	public static abstract /*final*/ class Timeout : Object
	{
		public const Int32 Infinite = -1;
	}

	public delegate void TimerCallback (Object sender); // todo

	// Generated from /Users/mh/Xcode/DerivedData/Fire-beiaefoboptwvtbxtvecylpnprxy/Build/Products/Debug/Fire.app/Contents/Resources/Mono/lib/mono/2.0/mscorlib.dll
	//[ComVisibleAttribute(true)]
	public /*final*/ class Timer : MarshalByRefObject, IDisposable
	{
		/*private static Timer.Scheduler scheduler;
		private TimerCallback callback;
		private Object state;
		private Int64 due_time_ms;
		private Int64 period_ms;
		private Int64 next_run;
		private Boolean disposed;
		private static const Int64 MaxValue = 4294967294;
		private static this .cctor();
		public Timer(TimerCallback callback);
		[CLSCompliantAttribute(false)]*/
		public Timer(TimerCallback callback, Object state, UInt32 dueTime, UInt32 period) {}
		public Timer(TimerCallback callback, Object state, TimeSpan dueTime, TimeSpan period) {}
		public Timer(TimerCallback callback, Object state, Int64 dueTime, Int64 period) {}
		public Timer(TimerCallback callback, Object state, Int32 dueTime, Int32 period) {}
		/*private void Init(TimerCallback callback, Object state, Int64 dueTime, Int64 period);
		private Boolean Change(Int64 dueTime, Int64 period, Boolean first);
		public Boolean Change(Int64 dueTime, Int64 period);
		[CLSCompliantAttribute(false)]*/
		public Boolean Change(UInt32 dueTime, UInt32 period) {}
		public Boolean Change(TimeSpan dueTime, TimeSpan period) {}
		public Boolean Change(Int32 dueTime, Int32 period) {}
		//public Boolean Dispose(WaitHandle notifyObject);*/
		public new void Dispose() {}
	}

	// Generated from /Users/mh/Xcode/DerivedData/Fire-beiaefoboptwvtbxtvecylpnprxy/Build/Products/Debug/Fire.app/Contents/Resources/Mono/lib/mono/4.0/mscorlib.dll
	//[ComVisibleAttribute(true)]
	public abstract class WaitHandle : MarshalByRefObject, IDisposable
	{
		/*public static const Int32 WaitTimeout = 258;
		private SafeWaitHandle safe_wait_handle;
		protected static IntPtr InvalidHandle;
		private Boolean disposed;
		private static this .cctor();
		protected WaitHandle();
		private static Boolean WaitAll_internal(WaitHandle[] handles, Int32 ms, Boolean exitContext);
		private static void CheckArray(WaitHandle[] handles, Boolean waitAll);
		public static Boolean WaitAll(WaitHandle[] waitHandles, TimeSpan timeout);
		public static Boolean WaitAll(WaitHandle[] waitHandles, Int32 millisecondsTimeout);
		public static Boolean WaitAll(WaitHandle[] waitHandles, TimeSpan timeout, Boolean exitContext);
		public static Boolean WaitAll(WaitHandle[] waitHandles, Int32 millisecondsTimeout, Boolean exitContext);
		public static Boolean WaitAll(WaitHandle[] waitHandles);
		private static Int32 WaitAny_internal(WaitHandle[] handles, Int32 ms, Boolean exitContext);
		[ReliabilityContractAttribute(3, 1)]
		public static Int32 WaitAny(WaitHandle[] waitHandles, TimeSpan timeout, Boolean exitContext);
		[ReliabilityContractAttribute(3, 1)]
		public static Int32 WaitAny(WaitHandle[] waitHandles, Int32 millisecondsTimeout);
		[ReliabilityContractAttribute(3, 1)]
		public static Int32 WaitAny(WaitHandle[] waitHandles, TimeSpan timeout);
		[ReliabilityContractAttribute(3, 1)]
		public static Int32 WaitAny(WaitHandle[] waitHandles, Int32 millisecondsTimeout, Boolean exitContext);
		[ReliabilityContractAttribute(3, 1)]
		public static Int32 WaitAny(WaitHandle[] waitHandles);
		public virtual void Close();
		protected virtual void Dispose(Boolean explicitDisposing);*/
		public new void Dispose() {}
		/*private Boolean WaitOne_internal(IntPtr handle, Int32 ms, Boolean exitContext);
		public static Boolean SignalAndWait(WaitHandle toSignal, WaitHandle toWaitOn, TimeSpan timeout, Boolean exitContext);
		public static Boolean SignalAndWait(WaitHandle toSignal, WaitHandle toWaitOn, Int32 millisecondsTimeout, Boolean exitContext);
		public static Boolean SignalAndWait(WaitHandle toSignal, WaitHandle toWaitOn);
		private static Boolean SignalAndWait_Internal(IntPtr toSignal, IntPtr toWaitOn, Int32 ms, Boolean exitContext);
		public virtual Boolean WaitOne(TimeSpan timeout, Boolean exitContext);
		public virtual Boolean WaitOne(TimeSpan timeout);
		public virtual Boolean WaitOne(Int32 millisecondsTimeout);
		public virtual Boolean WaitOne(Int32 millisecondsTimeout, Boolean exitContext);
		public virtual Boolean WaitOne();
		internal void CheckDisposed();
		[ObsoleteAttribute("In the profiles > 2.x, use SafeHandle instead of Handle")]
		public virtual IntPtr Handle { get; set; }
		public SafeWaitHandle SafeWaitHandle { get; set; }*/
	}
	#endif
}