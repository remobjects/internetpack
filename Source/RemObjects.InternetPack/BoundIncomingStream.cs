/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

namespace RemObjects.InternetPack
{
	public class BoundIncomingStream : Stream
	{
		private readonly Connection fConnection;
		private readonly Int32 fSize;
		private Int32 fDataLeft;

		public BoundIncomingStream(Connection connection, Int32 size)
		{
			this.fConnection = connection;
			this.fSize = size;
			this.fDataLeft = size;
		}

		#region System.IO.Stream Methods
		public override void Flush()
		{
			Byte[] lBuffer = new Byte[256];
			while (this.fDataLeft > 0)
				this.fDataLeft -= this.Read(lBuffer, 0, lBuffer.Length);
		}

		public override void Close()
		{
			this.Flush();
		}

		public override Int32 Read(Byte[] buffer, Int32 offset, Int32 count)
		{
			if (count > this.fDataLeft)
				count = this.fDataLeft;

			if (count <= 0)
				return 0;

			Int32 lResult = this.fConnection.Receive(buffer, offset, count);
			this.fDataLeft -= lResult;

			return lResult;
		}

		public override Int64 Seek(Int64 offset, PlatformSeekOrigin origin)
		{
			throw new Exception(String.Format("{0} does not support Seek", this.GetType().FullName));
		}

		public override void SetLength(Int64 length)
		{
			throw new Exception(String.Format("{0} does not support SetLength", this.GetType().FullName));
		}

		public override void Write(Byte[] buffer, Int32 offset, Int32 count)
		{
			throw new Exception(String.Format("{0} is a read-only Stream", this.GetType().FullName));
		}

		public override Boolean CanRead
		{
			get
			{
				return true;
			}
		}

		public override Boolean CanSeek
		{
			get
			{
				return false;
			}
		}

		public override Boolean CanWrite
		{
			get
			{
				return false;
			}
		}

		public override Int64 Length
		{
			get
			{
				return this.fSize;
			}
		}

		public override Int64 Position
		{
			get
			{
				return this.fSize - this.fDataLeft;
			}
			set
			{
				throw new Exception(String.Format("{0} does not support Seek", this.GetType().FullName));
			}
		}
		#endregion
	}
}