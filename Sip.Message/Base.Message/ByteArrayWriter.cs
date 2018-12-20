using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Base.Message
{
	public abstract class ByteArrayWriter
	{
		protected int end;

		protected int begin;

		protected ArraySegment<byte> segment;

		public ArraySegment<byte> Segment
		{
			get
			{
				return this.segment;
			}
		}

		public int Count
		{
			get
			{
				return this.end - this.begin;
			}
		}

		public int Offset
		{
			get
			{
				return this.segment.Offset + this.begin;
			}
		}

		public int End
		{
			get
			{
				return this.segment.Offset + this.end;
			}
		}

		public int OffsetOffset
		{
			get
			{
				return this.begin;
			}
		}

		public byte[] Buffer
		{
			get
			{
				return this.segment.Array;
			}
		}

		public ByteArrayWriter(ArraySegment<byte> segment) : this(0, segment)
		{
		}

		public ByteArrayWriter(int reservAtBegin, ArraySegment<byte> segment)
		{
			this.begin = reservAtBegin;
			this.end = reservAtBegin;
			this.segment = segment;
		}

		protected abstract void Reallocate(ref ArraySegment<byte> segment, int extraSize);

		public ArraySegment<byte> Detach()
		{
			ArraySegment<byte> result = this.segment;
			this.segment = default(ArraySegment<byte>);
			return result;
		}

		public void AddCount(int length)
		{
			this.end += length;
		}

		public ByteArrayPart ToByteArrayPart()
		{
			return new ByteArrayPart
			{
				Bytes = this.segment.Array,
				Begin = this.segment.Offset + this.begin,
				End = this.segment.Offset + this.end
			};
		}

		public void Write(ByteArrayPart part)
		{
			this.ValidateCapacity(part.Length);
			System.Buffer.BlockCopy(part.Bytes, part.Offset, this.segment.Array, this.segment.Offset + this.end, part.Length);
			this.end += part.Length;
		}

		public void Write(ArraySegment<byte> source)
		{
			if (source.Count > 0 && source.Array != null)
			{
				this.ValidateCapacity(source.Count);
				System.Buffer.BlockCopy(source.Array, source.Offset, this.segment.Array, this.segment.Offset + this.end, source.Count);
				this.end += source.Count;
			}
		}

		public void Write(string value)
		{
			int byteCount = Encoding.UTF8.GetByteCount(value);
			this.ValidateCapacity(byteCount);
			Encoding.UTF8.GetBytes(value, 0, value.Length, this.segment.Array, this.segment.Offset + this.end);
			this.end += byteCount;
		}

		public void Write(int value)
		{
			this.ValidateCapacity(11);
			if (value < 0)
			{
				this.segment.Array[this.segment.Offset + this.end++] = 45;
				this.Write((uint)(-(uint)value));
				return;
			}
			this.Write((uint)value);
		}

		public void Write(uint value)
		{
			this.ValidateCapacity(10);
			bool flag = false;
			for (uint num = 1000000000u; num >= 10u; num /= 10u)
			{
				byte b = (byte)(value / num);
				if (flag = (flag || b > 0))
				{
					this.segment.Array[this.segment.Offset + this.end++] = 48 + b;
				}
				value %= num;
			}
			this.segment.Array[this.segment.Offset + this.end++] = (byte)(48u + value);
		}

		public void Write(byte value)
		{
			this.ValidateCapacity(3);
			bool flag = false;
			for (byte b = 100; b >= 10; b /= 10)
			{
				byte b2 = value / b;
				if (flag = (flag || b2 > 0))
				{
					this.segment.Array[this.segment.Offset + this.end++] = 48 + b2;
				}
				value %= b;
			}
			this.segment.Array[this.segment.Offset + this.end++] = 48 + value;
		}

		public void Write(byte[] bytes)
		{
			this.ValidateCapacity(bytes.Length);
			System.Buffer.BlockCopy(bytes, 0, this.segment.Array, this.segment.Offset + this.end, bytes.Length);
			this.end += bytes.Length;
		}

		public void Write(IPEndPoint endpoint)
		{
			this.Write(endpoint.Address);
			this.segment.Array[this.segment.Offset + this.end++] = 58;
			this.Write(endpoint.Port);
		}

		public void Write(IPAddress address)
		{
			if (address.AddressFamily == AddressFamily.InterNetwork)
			{
				byte[] addressBytes = address.GetAddressBytes();
				this.Write(addressBytes[0]);
				this.segment.Array[this.segment.Offset + this.end++] = 46;
				this.Write(addressBytes[1]);
				this.segment.Array[this.segment.Offset + this.end++] = 46;
				this.Write(addressBytes[2]);
				this.segment.Array[this.segment.Offset + this.end++] = 46;
				this.Write(addressBytes[3]);
				return;
			}
			this.Write(Encoding.UTF8.GetBytes(address.ToString()));
		}

		public void WriteAsHex8(int value)
		{
			this.ValidateCapacity(8);
			this.end += 8;
			int i = 1;
			while (i < 9)
			{
				this.segment.Array[this.segment.Offset + this.end - i] = ByteArrayWriter.GetLowerHexChar((byte)(value & 15));
				i++;
				value >>= 4;
			}
		}

		public void WriteAsHex(ArraySegment<byte> data)
		{
			this.ValidateCapacity(data.Count * 2);
			for (int i = 0; i < data.Count; i++)
			{
				this.segment.Array[this.segment.Offset + this.end] = ByteArrayWriter.GetLowerHexChar((byte)(data.Array[data.Offset + i] >> 4));
				this.end++;
				this.segment.Array[this.segment.Offset + this.end] = ByteArrayWriter.GetLowerHexChar(data.Array[data.Offset + i] & 15);
				this.end++;
			}
		}

		public void WriteAsHex(byte data)
		{
			this.ValidateCapacity(2);
			this.segment.Array[this.segment.Offset + this.end] = ByteArrayWriter.GetLowerHexChar((byte)(data >> 4));
			this.end++;
			this.segment.Array[this.segment.Offset + this.end] = ByteArrayWriter.GetLowerHexChar(data & 15);
			this.end++;
		}

		public void WriteAsBase64(ArraySegment<byte> data)
		{
			this.ValidateCapacity(Base64Encoding.GetEncodedLength(data.Count));
			this.end += Base64Encoding.Encode(data, this.segment.Array, this.segment.Offset + this.end);
		}

		public ArraySegment<byte> GetBytesForCustomWrite(int size)
		{
			this.ValidateCapacity(size);
			this.end += size;
			return new ArraySegment<byte>(this.segment.Array, this.segment.Offset + this.end - size, this.segment.Offset + this.end);
		}

		public void ValidateCapacity(int extraSize)
		{
			if (this.end + extraSize > this.segment.Count)
			{
				this.Reallocate(ref this.segment, extraSize);
			}
		}

		public static byte GetLowerHexChar(byte digit)
		{
			switch (digit)
			{
			case 0:
				return 48;
			case 1:
				return 49;
			case 2:
				return 50;
			case 3:
				return 51;
			case 4:
				return 52;
			case 5:
				return 53;
			case 6:
				return 54;
			case 7:
				return 55;
			case 8:
				return 56;
			case 9:
				return 57;
			case 10:
				return 97;
			case 11:
				return 98;
			case 12:
				return 99;
			case 13:
				return 100;
			case 14:
				return 101;
			case 15:
				return 102;
			default:
				throw new ArgumentOutOfRangeException("digit");
			}
		}

		public void WriteToTop(ByteArrayPart part)
		{
			this.WriteToTop(part, -1);
		}

		public void WriteToTop(ByteArrayPart part, int ignoreAfter)
		{
			int num = (ignoreAfter > 0 && ignoreAfter < part.Length) ? ignoreAfter : part.Length;
			this.ValidateCapacityToTop(num);
			this.begin -= num;
			System.Buffer.BlockCopy(part.Bytes, part.Offset, this.segment.Array, this.segment.Offset + this.begin, num);
		}

		public void WriteToTop(int value)
		{
			this.ValidateCapacityToTop(11);
			if (value < 0)
			{
				this.WriteToTop((uint)(-(uint)value));
				this.segment.Array[this.segment.Offset + --this.begin] = 45;
				return;
			}
			this.WriteToTop((uint)value);
		}

		public void WriteToTop(uint value)
		{
			this.ValidateCapacityToTop(10);
			this.ReversWrite(value, ref this.begin);
		}

		protected void ReversWrite(uint value, ref int position)
		{
			do
			{
				byte b = (byte)(value % 10u);
				this.segment.Array[this.segment.Offset + --position] = 48 + b;
				value /= 10u;
			}
			while (value > 0u);
		}

		public void ValidateCapacityToTop(int extraSize)
		{
			if (this.begin < extraSize)
			{
				throw new ArgumentOutOfRangeException("Not enougth space was reserved at begin");
			}
		}

		public void Write(byte[] part1, byte[] part2)
		{
			this.Write(part1);
			this.Write(part2);
		}

		public void Write(byte[] part1, byte[] part2, byte[] part3)
		{
			this.Write(part1);
			this.Write(part2);
			this.Write(part3);
		}

		public void Write(byte[] part1, byte[] part2, byte[] part3, byte[] part4)
		{
			this.Write(part1);
			this.Write(part2);
			this.Write(part3);
			this.Write(part4);
		}

		public void Write(byte[] part1, byte[] part2, byte[] part3, byte[] part4, byte[] part5)
		{
			this.Write(part1);
			this.Write(part2);
			this.Write(part3);
			this.Write(part4);
			this.Write(part5);
		}

		public void Write(byte[] part1, byte[] part2, byte[] part3, byte[] part4, byte[] part5, byte[] part6)
		{
			this.Write(part1);
			this.Write(part2);
			this.Write(part3);
			this.Write(part4);
			this.Write(part5);
			this.Write(part6);
		}

		public void Write(ByteArrayPart part1, ByteArrayPart part2)
		{
			this.Write(part1);
			this.Write(part2);
		}

		public void Write(ByteArrayPart part1, int part2)
		{
			this.Write(part1);
			this.Write(part2);
		}

		public void Write(ByteArrayPart part1, ByteArrayPart part2, ByteArrayPart part3)
		{
			this.Write(part1);
			this.Write(part2);
			this.Write(part3);
		}

		public void Write(ByteArrayPart part1, ByteArrayPart part2, int part3)
		{
			this.Write(part1);
			this.Write(part2);
			this.Write(part3);
		}

		public void Write(ByteArrayPart part1, int part2, ByteArrayPart part3)
		{
			this.Write(part1);
			this.Write(part2);
			this.Write(part3);
		}

		public void Write(ByteArrayPart part1, ByteArrayPart part2, ByteArrayPart part3, ByteArrayPart part4)
		{
			this.Write(part1);
			this.Write(part2);
			this.Write(part3);
			this.Write(part4);
		}

		public void Write(ByteArrayPart part1, ByteArrayPart part2, int part3, ByteArrayPart part4)
		{
			this.Write(part1);
			this.Write(part2);
			this.Write(part3);
			this.Write(part4);
		}

		public void Write(ByteArrayPart part1, ByteArrayPart part2, ByteArrayPart part3, int part4)
		{
			this.Write(part1);
			this.Write(part2);
			this.Write(part3);
			this.Write(part4);
		}

		public void Write(ByteArrayPart part1, ByteArrayPart part2, ByteArrayPart part3, IPAddress part4)
		{
			this.Write(part1);
			this.Write(part2);
			this.Write(part3);
			this.Write(part4);
		}

		public void Write(ByteArrayPart part1, ByteArrayPart part2, ByteArrayPart part3, int part4, ByteArrayPart part5)
		{
			this.Write(part1);
			this.Write(part2);
			this.Write(part3);
			this.Write(part4);
			this.Write(part5);
		}

		public void Write(ByteArrayPart part1, ByteArrayPart part2, ByteArrayPart part3, ByteArrayPart part4, ByteArrayPart part5)
		{
			this.Write(part1);
			this.Write(part2);
			this.Write(part3);
			this.Write(part4);
			this.Write(part5);
		}

		public void Write(ByteArrayPart part1, ByteArrayPart part2, ByteArrayPart part3, ByteArrayPart part4, ByteArrayPart part5, ByteArrayPart part6)
		{
			this.Write(part1);
			this.Write(part2);
			this.Write(part3);
			this.Write(part4);
			this.Write(part5);
			this.Write(part6);
		}

		public void Write(ByteArrayPart part1, ByteArrayPart part2, int part3, ByteArrayPart part4, ByteArrayPart part5, ByteArrayPart part6)
		{
			this.Write(part1);
			this.Write(part2);
			this.Write(part3);
			this.Write(part4);
			this.Write(part5);
			this.Write(part6);
		}

		public void Write(ByteArrayPart part1, ByteArrayPart part2, ByteArrayPart part3, int part4, ByteArrayPart part5, ByteArrayPart part6)
		{
			this.Write(part1);
			this.Write(part2);
			this.Write(part3);
			this.Write(part4);
			this.Write(part5);
			this.Write(part6);
		}

		public void Write(ByteArrayPart part1, ByteArrayPart part2, ByteArrayPart part3, int part4, ByteArrayPart part5, ByteArrayPart part6, ByteArrayPart part7)
		{
			this.Write(part1);
			this.Write(part2);
			this.Write(part3);
			this.Write(part4);
			this.Write(part5);
			this.Write(part6);
			this.Write(part7);
		}

		public void Write(ByteArrayPart part1, ByteArrayPart part2, ByteArrayPart part3, ByteArrayPart part4, ByteArrayPart part5, ByteArrayPart part6, ByteArrayPart part7)
		{
			this.Write(part1);
			this.Write(part2);
			this.Write(part3);
			this.Write(part4);
			this.Write(part5);
			this.Write(part6);
			this.Write(part7);
		}

		public void Write(ByteArrayPart part1, ByteArrayPart part2, ByteArrayPart part3, ByteArrayPart part4, ByteArrayPart part5, ByteArrayPart part6, ByteArrayPart part7, ByteArrayPart part8)
		{
			this.Write(part1);
			this.Write(part2);
			this.Write(part3);
			this.Write(part4);
			this.Write(part5);
			this.Write(part6);
			this.Write(part7);
			this.Write(part8);
		}

		public void Write(ByteArrayPart part1, ByteArrayPart part2, ByteArrayPart part3, ByteArrayPart part4, ByteArrayPart part5, ByteArrayPart part6, ByteArrayPart part7, ByteArrayPart part8, ByteArrayPart part9)
		{
			this.Write(part1);
			this.Write(part2);
			this.Write(part3);
			this.Write(part4);
			this.Write(part5);
			this.Write(part6);
			this.Write(part7);
			this.Write(part8);
			this.Write(part9);
		}

		public override string ToString()
		{
			return Encoding.UTF8.GetString(this.segment.Array, this.segment.Offset + this.begin, this.Count);
		}
	}
}
