namespace RemObjects.InternetPack
{
	// Generated from /Users/mh/Xcode/DerivedData/Fire-beiaefoboptwvtbxtvecylpnprxy/Build/Products/Debug/Fire.app/Contents/Resources/Mono/lib/mono/2.0/mscorlib.dll
	//[ComVisibleAttribute(true)]
	public static abstract /*final*/ class Timeout : Object
	{
		public const Int32 Infinite = -1;
	}

	public delegate void TimerCallback (Object state);

	// Generated from /Users/mh/Xcode/DerivedData/Fire-beiaefoboptwvtbxtvecylpnprxy/Build/Products/Debug/Fire.app/Contents/Resources/Mono/lib/mono/2.0/mscorlib.dll
	//[ComVisibleAttribute(true)]
	public /*final*/ class Timer : MarshalByRefObject, IDisposable
	{
		private readonly Monitor fLock = new Monitor();
		private RemObjects.Elements.RTL.Timer fTimer;
		private TimerCallback fCallback;
		private object fState;

		private void StopTimer()
		{
			var lTimer = fTimer;
			if (lTimer != null && lTimer.Enabled)
				lTimer.Stop();
		}

		private void SetupTimer(Int32 dueTime, Int32 period)
		{
			if (dueTime != Timeout.Infinite)
			{
				if ((period == 0) || (period == Timeout.Infinite))
				{
					fTimer = new RemObjects.Elements.RTL.Timer(dueTime, false, timer => fCallback(fState));
					fTimer.Start();
				}
				else
				{
					fTimer = new RemObjects.Elements.RTL.Timer(period, false, timer => fCallback(fState));
					fTimer.Start();
				}
			}
			else
			{
				fTimer = null;
			}
		}

		public Timer(TimerCallback callback, Object state, Int32 dueTime, Int32 period)
		{
			fCallback = callback;
			fState = state;
			SetupTimer(dueTime, period);
		}

		public Boolean Change(Int32 dueTime, Int32 period)
		{
			lock (fLock)
			{
				StopTimer();
				SetupTimer(dueTime, period);
			}

			return true;
		}

		public void Dispose()
		{
			lock (fLock)
			{
				StopTimer();
				fTimer = null;
			}
		}
	}
	#if !ECHOES
	// Generated from /Users/mh/Xcode/DerivedData/Fire-beiaefoboptwvtbxtvecylpnprxy/Build/Products/Debug/Fire.app/Contents/Resources/Mono/lib/mono/4.0/mscorlib.dll
	//[ComVisibleAttribute(true)]
	public abstract class WaitHandle : MarshalByRefObject, IDisposable
	{
		//public static const Int32 WaitTimeout = 258;
		public new void Dispose() {}
		public abstract Boolean WaitOne();
	}

	public class EventWaitHandle: WaitHandle
	{
		#if island
		private RemObjects.Elements.System.EventWaitHandle fHandle;
		#else
		private Monitor fHandle;
		#endif

		public EventWaitHandle()
		{
			#if island
			fHandle = new RemObjects.Elements.System.EventWaitHandle(false, false);
			#else
			fHandle = new Monitor();
			#endif
		}

		public override Boolean WaitOne()
		{
			#if island
			fHandle.Wait();
			#else
			fHandle.Lock();
			#endif
			return true;
		}

		public Boolean Set()
		{
			#if island
			fHandle.Set();
			#else
			fHandle.Unlock();
			#endif
			return true;
		}

		#if !cooper
		public new void Dispose()
		{

		}
		#endif
	}
	#endif
}
