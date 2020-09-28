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
		private RemObjects.Elements.RTL.Timer fTimer;
		private TimerCallback fCallback;
		private object fState;

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
				fTimer?.Stop();
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
			if (fTimer?.Enabled)
				fTimer.Stop();

			SetupTimer(dueTime, period);
			return true;
		}

		public void Dispose()
		{
			if (fTimer?.Enabled)
				fTimer.Stop();
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
		private Monitor fHandle;

		public EventWaitHandle()
		{
			fHandle = new Monitor();
		}

		public override Boolean WaitOne()
		{
			fHandle.Lock();
			return true;
		}

		public Boolean Set()
		{
			fHandle.Unlock();
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