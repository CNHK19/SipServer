using System;

namespace SocketServers
{
	public abstract class HeaderContentConnection : BaseConnection
	{
		private enum StreamState
		{
			WaitingHeaders,
			WaitingHeadersContinue,
			WaitingMicroBody,
			WaitingSmallBody,
			WaitingBigBody
		}

		private enum Storage
		{
			None,
			E,
			E1,
			Buffer1,
			Buffer2
		}

		protected enum ParseCode
		{
			NotEnoughData,
			HeaderDone,
			Error,
			Skip
		}

		protected struct ParseResult
		{
			public HeaderContentConnection.ParseCode ParseCode;

			public int HeaderLength;

			public int ContentLength;

			public int Count
			{
				get
				{
					return this.HeaderLength;
				}
			}

			public ParseResult(HeaderContentConnection.ParseCode parseCode, int headerLength, int contentLength)
			{
				this.ParseCode = parseCode;
				this.HeaderLength = headerLength;
				this.ContentLength = contentLength;
			}

			public static HeaderContentConnection.ParseResult HeaderDone(int headerLength, int contentLength)
			{
				return new HeaderContentConnection.ParseResult(HeaderContentConnection.ParseCode.HeaderDone, headerLength, contentLength);
			}

			public static HeaderContentConnection.ParseResult Skip(int count)
			{
				return new HeaderContentConnection.ParseResult(HeaderContentConnection.ParseCode.Skip, count, 0);
			}

			public static HeaderContentConnection.ParseResult Error()
			{
				return new HeaderContentConnection.ParseResult
				{
					ParseCode = HeaderContentConnection.ParseCode.Error
				};
			}

			public static HeaderContentConnection.ParseResult NotEnoughData()
			{
				return new HeaderContentConnection.ParseResult
				{
					ParseCode = HeaderContentConnection.ParseCode.NotEnoughData
				};
			}
		}

		protected enum ResetReason
		{
			NotEnoughData,
			ResetStateCalled
		}

		public const int MaximumHeadersSize = 8192;

		private const int MinimumBuffer1Size = 4096;

		private ServerAsyncEventArgs e1;

		private StreamBuffer buffer1;

		private StreamBuffer buffer2;

		private HeaderContentConnection.StreamState state;

		private int expectedContentLength;

		private int receivedContentLength;

		private int buffer1UnusedCount;

		private ArraySegment<byte> headerData;

		private ArraySegment<byte> contentData;

		private int bytesProccessed;

		private bool ready;

		private HeaderContentConnection.Storage readerStorage;

		private HeaderContentConnection.Storage contentStorage;

		private int keepAliveRecived;

		private StreamBuffer Buffer1
		{
			get
			{
				if (this.buffer1 == null)
				{
					this.buffer1 = new StreamBuffer();
				}
				return this.buffer1;
			}
		}

		private StreamBuffer Buffer2
		{
			get
			{
				if (this.buffer2 == null)
				{
					this.buffer2 = new StreamBuffer();
				}
				return this.buffer2;
			}
		}

		public ArraySegment<byte> Header
		{
			get
			{
				return this.headerData;
			}
		}

		public bool IsMessageReady
		{
			get
			{
				return this.ready;
			}
		}

		public ArraySegment<byte> Content
		{
			get
			{
				return this.contentData;
			}
		}

		public HeaderContentConnection()
		{
			this.state = HeaderContentConnection.StreamState.WaitingHeaders;
		}

		public new void Dispose()
		{
			base.Dispose();
			if (this.buffer1 != null)
			{
				this.buffer1.Dispose();
			}
			if (this.buffer2 != null)
			{
				this.buffer2.Dispose();
			}
			if (this.e1 != null)
			{
				this.e1.Dispose();
				this.e1 = null;
			}
		}

		public void ResetState()
		{
			this.ready = false;
			this.headerData = default(ArraySegment<byte>);
			this.expectedContentLength = 0;
			this.receivedContentLength = 0;
			this.contentData = default(ArraySegment<byte>);
			this.readerStorage = HeaderContentConnection.Storage.None;
			this.contentStorage = HeaderContentConnection.Storage.None;
			this.ResetParser(HeaderContentConnection.ResetReason.ResetStateCalled);
			if (this.buffer1 != null)
			{
				this.buffer1UnusedCount = ((this.buffer1.Count <= 0) ? (this.buffer1UnusedCount + 1) : 0);
				if (this.buffer1.Capacity <= 8192 && this.buffer1UnusedCount < 8)
				{
					this.buffer1.Clear();
				}
				else
				{
					this.buffer1.Free();
				}
			}
			if (this.buffer2 != null)
			{
				this.buffer2.Free();
			}
			if (this.e1 != null)
			{
				this.e1.Dispose();
				this.e1 = null;
			}
			this.keepAliveRecived = 0;
			this.state = HeaderContentConnection.StreamState.WaitingHeaders;
		}

		public bool Proccess(ref ServerAsyncEventArgs e, out bool closeConnection)
		{
			closeConnection = false;
			switch (this.state)
			{
			case HeaderContentConnection.StreamState.WaitingHeaders:
			{
				int num = this.bytesProccessed;
				ArraySegment<byte> data = new ArraySegment<byte>(e.Buffer, e.Offset + this.bytesProccessed, e.BytesTransferred - this.bytesProccessed);
				this.PreProcessRaw(data);
				HeaderContentConnection.ParseResult parseResult = this.Parse(data);
				switch (parseResult.ParseCode)
				{
				case HeaderContentConnection.ParseCode.NotEnoughData:
					this.bytesProccessed += data.Count;
					this.ResetParser(HeaderContentConnection.ResetReason.NotEnoughData);
					this.Buffer1.Resize(8192);
					this.Buffer1.CopyTransferredFrom(e, num);
					this.state = HeaderContentConnection.StreamState.WaitingHeadersContinue;
					break;
				case HeaderContentConnection.ParseCode.HeaderDone:
					this.bytesProccessed += parseResult.HeaderLength;
					this.SetReaderStorage(HeaderContentConnection.Storage.E, e.Buffer, e.Offset + num, parseResult.HeaderLength);
					this.expectedContentLength = parseResult.ContentLength;
					if (this.expectedContentLength <= 0)
					{
						this.SetReady();
					}
					else
					{
						int num2 = e.BytesTransferred - this.bytesProccessed;
						if (num2 >= this.expectedContentLength)
						{
							this.SetReady(HeaderContentConnection.Storage.E, e.Buffer, e.Offset + this.bytesProccessed, this.expectedContentLength);
							this.bytesProccessed += this.expectedContentLength;
						}
						else
						{
							if (this.expectedContentLength <= e.Count - e.BytesTransferred)
							{
								this.state = HeaderContentConnection.StreamState.WaitingMicroBody;
							}
							else if (this.expectedContentLength < 8192)
							{
								if ((this.Buffer1.IsInvalid || this.Buffer1.Capacity < this.expectedContentLength) && !this.Buffer1.Resize(Math.Max(this.expectedContentLength, 4096)))
								{
									closeConnection = true;
								}
								if (!closeConnection)
								{
									this.Buffer1.CopyTransferredFrom(e, this.bytesProccessed);
									this.state = HeaderContentConnection.StreamState.WaitingSmallBody;
								}
							}
							else if (!this.Buffer2.Resize(this.expectedContentLength))
							{
								closeConnection = true;
							}
							else
							{
								this.Buffer2.CopyTransferredFrom(e, this.bytesProccessed);
								this.state = HeaderContentConnection.StreamState.WaitingBigBody;
							}
							if (!closeConnection)
							{
								this.e1 = e;
								e = null;
								this.readerStorage = HeaderContentConnection.Storage.E1;
							}
							this.bytesProccessed += num2;
							this.receivedContentLength += num2;
						}
					}
					break;
				case HeaderContentConnection.ParseCode.Error:
					closeConnection = true;
					break;
				case HeaderContentConnection.ParseCode.Skip:
					this.bytesProccessed += parseResult.Count;
					break;
				}
				break;
			}
			case HeaderContentConnection.StreamState.WaitingHeadersContinue:
			{
				int num3 = Math.Min(e.BytesTransferred - this.bytesProccessed, this.Buffer1.FreeSize);
				this.PreProcessRaw(new ArraySegment<byte>(e.Buffer, e.Offset, e.BytesTransferred - this.bytesProccessed));
				Buffer.BlockCopy(e.Buffer, e.Offset, this.Buffer1.Array, this.Buffer1.Offset + this.Buffer1.Count, num3);
				ArraySegment<byte> data2 = new ArraySegment<byte>(this.Buffer1.Array, this.Buffer1.Offset, this.Buffer1.Count + num3);
				HeaderContentConnection.ParseResult parseResult2 = this.Parse(data2);
				switch (parseResult2.ParseCode)
				{
				case HeaderContentConnection.ParseCode.NotEnoughData:
					this.ResetParser(HeaderContentConnection.ResetReason.NotEnoughData);
					if (data2.Count < this.Buffer1.Capacity)
					{
						this.Buffer1.AddCount(num3);
						this.bytesProccessed += num3;
					}
					else
					{
						closeConnection = true;
					}
					break;
				case HeaderContentConnection.ParseCode.HeaderDone:
				{
					int num4 = parseResult2.HeaderLength - this.Buffer1.Count;
					this.Buffer1.AddCount(num4);
					this.bytesProccessed += num4;
					this.SetReaderStorage(HeaderContentConnection.Storage.Buffer1, this.Buffer1.Array, this.Buffer1.Offset, parseResult2.HeaderLength);
					this.expectedContentLength = parseResult2.ContentLength;
					if (this.expectedContentLength <= 0)
					{
						this.SetReady();
					}
					else
					{
						int num5 = e.BytesTransferred - this.bytesProccessed;
						if (num5 >= this.expectedContentLength)
						{
							this.SetReady(HeaderContentConnection.Storage.E, e.Buffer, e.Offset + this.bytesProccessed, this.expectedContentLength);
							this.bytesProccessed += this.expectedContentLength;
						}
						else
						{
							if (this.expectedContentLength < this.Buffer1.FreeSize)
							{
								this.Buffer1.AddCount(num5);
								this.state = HeaderContentConnection.StreamState.WaitingSmallBody;
							}
							else
							{
								if (!this.Buffer2.Resize(this.expectedContentLength))
								{
									closeConnection = true;
								}
								this.Buffer2.CopyTransferredFrom(e, this.bytesProccessed);
								this.state = HeaderContentConnection.StreamState.WaitingBigBody;
							}
							this.bytesProccessed += num5;
							this.receivedContentLength += num5;
						}
					}
					break;
				}
				case HeaderContentConnection.ParseCode.Error:
					closeConnection = true;
					break;
				case HeaderContentConnection.ParseCode.Skip:
					throw new NotImplementedException();
				}
				break;
			}
			case HeaderContentConnection.StreamState.WaitingMicroBody:
			{
				int num6 = Math.Min(e.BytesTransferred - this.bytesProccessed, this.expectedContentLength - this.receivedContentLength);
				ArraySegment<byte> data3 = new ArraySegment<byte>(e.Buffer, e.Offset + this.bytesProccessed, num6);
				this.PreProcessRaw(data3);
				Buffer.BlockCopy(data3.Array, data3.Offset, this.e1.Buffer, this.e1.Offset + this.e1.BytesTransferred, data3.Count);
				this.e1.BytesTransferred += num6;
				this.receivedContentLength += num6;
				this.bytesProccessed += num6;
				if (this.receivedContentLength == this.expectedContentLength)
				{
					this.SetReady(HeaderContentConnection.Storage.E1, this.e1.Buffer, this.e1.Offset + this.e1.BytesTransferred - this.receivedContentLength, this.receivedContentLength);
				}
				break;
			}
			case HeaderContentConnection.StreamState.WaitingSmallBody:
			{
				int num7 = Math.Min(e.BytesTransferred - this.bytesProccessed, this.expectedContentLength - this.receivedContentLength);
				ArraySegment<byte> arraySegment = new ArraySegment<byte>(e.Buffer, e.Offset + this.bytesProccessed, num7);
				this.PreProcessRaw(arraySegment);
				this.Buffer1.CopyFrom(arraySegment);
				this.receivedContentLength += num7;
				this.bytesProccessed += num7;
				if (this.receivedContentLength == this.expectedContentLength)
				{
					this.SetReady(HeaderContentConnection.Storage.Buffer1, this.Buffer1.Array, this.Buffer1.Offset + this.Buffer1.Count - this.receivedContentLength, this.receivedContentLength);
				}
				break;
			}
			case HeaderContentConnection.StreamState.WaitingBigBody:
			{
				int num8 = Math.Min(e.BytesTransferred - this.bytesProccessed, this.expectedContentLength - this.receivedContentLength);
				ArraySegment<byte> arraySegment2 = new ArraySegment<byte>(e.Buffer, e.Offset + this.bytesProccessed, num8);
				this.PreProcessRaw(arraySegment2);
				this.Buffer2.CopyFrom(arraySegment2);
				this.receivedContentLength += num8;
				this.bytesProccessed += num8;
				if (this.receivedContentLength == this.expectedContentLength)
				{
					this.SetReady(HeaderContentConnection.Storage.Buffer2, this.Buffer2.Array, this.Buffer2.Offset + this.Buffer2.Count - this.receivedContentLength, this.receivedContentLength);
				}
				break;
			}
			}
			bool flag = !closeConnection && e != null && this.bytesProccessed < e.BytesTransferred;
			if (!flag)
			{
				this.bytesProccessed = 0;
			}
			return flag;
		}

		public void Dettach(ref ServerAsyncEventArgs e, out ArraySegment<byte> segment1, out ArraySegment<byte> segment2)
		{
			if (this.readerStorage == HeaderContentConnection.Storage.E)
			{
				int num = this.headerData.Count;
				if (this.contentStorage == this.readerStorage)
				{
					num += this.contentData.Count;
				}
				segment1 = this.Detach(ref e, num);
				segment2 = ((this.contentStorage != this.readerStorage) ? this.Dettach(this.contentStorage) : default(ArraySegment<byte>));
				return;
			}
			segment1 = this.Dettach(this.readerStorage);
			if (this.contentStorage == this.readerStorage)
			{
				segment2 = default(ArraySegment<byte>);
				return;
			}
			if (this.contentStorage == HeaderContentConnection.Storage.E)
			{
				segment2 = this.Detach(ref e, this.contentData.Count);
				return;
			}
			segment2 = this.Dettach(this.contentStorage);
		}

		private ArraySegment<byte> Detach(ref ServerAsyncEventArgs e, int size)
		{
			ServerAsyncEventArgs serverAsyncEventArgs = null;
			if (e.BytesTransferred > size)
			{
				serverAsyncEventArgs = e.CreateDeepCopy();
			}
			ArraySegment<byte> result = e.DetachBuffer();
			EventArgsManager.Put(ref e);
			if (serverAsyncEventArgs != null)
			{
				e = serverAsyncEventArgs;
			}
			return result;
		}

		private ArraySegment<byte> Dettach(HeaderContentConnection.Storage storage)
		{
			switch (storage)
			{
			case HeaderContentConnection.Storage.None:
				return default(ArraySegment<byte>);
			case HeaderContentConnection.Storage.E1:
				return this.e1.DetachBuffer();
			case HeaderContentConnection.Storage.Buffer1:
				return this.buffer1.Detach();
			case HeaderContentConnection.Storage.Buffer2:
				return this.buffer2.Detach();
			}
			throw new ArgumentException();
		}

		protected abstract void ResetParser(HeaderContentConnection.ResetReason reason);

		protected abstract void MessageReady();

		protected abstract HeaderContentConnection.ParseResult Parse(ArraySegment<byte> data);

		protected abstract void PreProcessRaw(ArraySegment<byte> data);

		private void SetReaderStorage(HeaderContentConnection.Storage readerStorage1, byte[] buffer, int offset, int count)
		{
			this.readerStorage = readerStorage1;
			this.headerData = new ArraySegment<byte>(buffer, offset, count);
		}

		private void SetReady(HeaderContentConnection.Storage contentStorage1, byte[] buffer, int offset, int count)
		{
			this.contentStorage = contentStorage1;
			this.contentData = new ArraySegment<byte>(buffer, offset, count);
			this.ready = true;
			this.MessageReady();
		}

		private void SetReady()
		{
			this.contentStorage = HeaderContentConnection.Storage.None;
			this.ready = true;
			this.MessageReady();
		}
	}
}
