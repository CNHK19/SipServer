using Base.Message;
using System;
using System.Net;
using System.Text;

namespace Sip.Message
{
	public struct BeginEndIndex : IEquatable<BeginEndIndex>, IEquatable<ByteArrayPart>, IEquatable<byte[]>
	{
		private const byte Cl = 10;

		private const byte Cr = 13;

		private const byte Space = 32;

		private const byte Comma = 44;

		private int index;

		public int Begin;

		public int End;

		public byte[] Bytes
		{
			get
			{
				return SipMessageReader.bytes[this.index];
			}
		}

		public int Offset
		{
			get
			{
				return this.Begin;
			}
		}

		public int Length
		{
			get
			{
				if (!this.IsInvalid)
				{
					return this.End - this.Begin;
				}
				return 0;
			}
		}

		public bool IsValid
		{
			get
			{
				return this.Begin >= 0 && this.End >= 0;
			}
		}

		public bool IsInvalid
		{
			get
			{
				return this.Begin < 0 || this.End < 0;
			}
		}

		public bool IsNotEmpty
		{
			get
			{
				return this.Begin >= 0 && this.End >= 0 && this.Begin < this.End;
			}
		}

		public void SetDefaultValue(int index1)
		{
			this.SetDefaultValue();
			this.index = index1;
		}

		public void SetDefaultValue()
		{
			this.Begin = -2147483648;
			this.End = -2147483648;
		}

		public static implicit operator ByteArrayPart(BeginEndIndex be)
		{
			return new ByteArrayPart
			{
				Bytes = be.Bytes,
				Begin = be.Begin,
				End = be.End
			};
		}

		public void BlockCopyTo(byte[] bytes, int offset)
		{
			Buffer.BlockCopy(this.Bytes, this.Begin, bytes, offset, this.Length);
		}

		public void BlockCopyTo(byte[] bytes, ref int offset)
		{
			Buffer.BlockCopy(this.Bytes, this.Begin, bytes, offset, this.Length);
			offset += this.Length;
		}

		public bool StartsWith(byte[] bytes)
		{
			int num = bytes.Length;
			if (this.Length < num)
			{
				return false;
			}
			int num2 = 0;
			int num3 = num2 + num;
			int num4 = this.Begin;
			for (int i = num2; i < num3; i++)
			{
				if (this.Bytes[num4] != bytes[i])
				{
					return false;
				}
				num4++;
			}
			return true;
		}

		public bool EndWith(ByteArrayPart part)
		{
			int length = part.Length;
			if (this.Length < length)
			{
				return false;
			}
			int num = this.Begin + this.Length - part.Length;
			for (int i = part.Begin; i < part.End; i++)
			{
				if (this.Bytes[num] != part.Bytes[i])
				{
					return false;
				}
				num++;
			}
			return true;
		}

		public ByteArrayPart DeepCopy()
		{
			if (this.IsInvalid)
			{
				return ByteArrayPart.Invalid;
			}
			ByteArrayPart result = new ByteArrayPart
			{
				Bytes = new byte[this.Length],
				Begin = 0,
				End = this.Length
			};
			Buffer.BlockCopy(this.Bytes, this.Offset, result.Bytes, result.Offset, this.Length);
			return result;
		}

		public new string ToString()
		{
			if (this.Bytes == null || this.Begin < 0 || this.End < 0)
			{
				return null;
			}
			return Encoding.UTF8.GetString(this.Bytes, this.Offset, this.Length);
		}

		public IPAddress ToIPAddress()
		{
			IPAddress none = IPAddress.None;
			if (this.IsValid)
			{
				IPAddress.TryParse(this.ToString(), out none);
			}
			return none;
		}

		public ArraySegment<byte> ToArraySegment()
		{
			return new ArraySegment<byte>(this.Bytes, this.Offset, this.Length);
		}

		public bool Equals(BeginEndIndex y)
		{
			return this.Equals(y.Bytes, y.Begin, y.Length);
		}

		public bool Equals(ByteArrayPart y)
		{
			return this.Equals(y.Bytes, y.Begin, y.Length);
		}

		public bool Equals(byte[] bytes)
		{
			return this.Equals(bytes, 0, bytes.Length);
		}

		public static bool operator ==(BeginEndIndex x, BeginEndIndex y)
		{
			return x.Equals(y);
		}

		public static bool operator ==(BeginEndIndex x, ByteArrayPart y)
		{
			return x.Equals(y);
		}

		public static bool operator !=(BeginEndIndex x, BeginEndIndex y)
		{
			return !x.Equals(y);
		}

		public static bool operator !=(BeginEndIndex x, ByteArrayPart y)
		{
			return !x.Equals(y);
		}

		private bool Equals(byte[] bytes, int startIndex, int length)
		{
			if (this.Length != length)
			{
				return false;
			}
			int num = startIndex + length;
			int num2 = this.Begin;
			for (int i = startIndex; i < num; i++)
			{
				if (this.Bytes[num2] != bytes[i])
				{
					return false;
				}
				num2++;
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			return (obj is ByteArrayPart && this.Equals((ByteArrayPart)obj)) || (obj is BeginEndIndex && this.Equals((BeginEndIndex)obj)) || (obj is byte[] && this.Equals(obj as byte[]));
		}

		public override int GetHashCode()
		{
			int num = 0;
			int num2 = this.End - this.Begin - 1;
			if (num2 >= 0)
			{
				for (int i = 0; i <= 3; i++)
				{
					num <<= 8;
					num |= (int)this.Bytes[this.Begin + num2 * i / 3];
				}
			}
			return num;
		}

		public void TrimStartSws()
		{
			while (this.Begin < this.End && this.Bytes[this.Begin] == 32)
			{
				this.Begin++;
			}
			if (this.Begin + 2 < this.End && this.Bytes[this.Begin] == 13 && this.Bytes[this.Begin + 1] == 10 && this.Bytes[this.Begin + 2] == 32)
			{
				this.Begin += 3;
			}
			while (this.Begin < this.End && this.Bytes[this.Begin] == 32)
			{
				this.Begin++;
			}
		}

		public void TrimEndSws()
		{
			if (this.Begin < this.End && this.Bytes[this.End - 1] == 32)
			{
				while (this.Begin < this.End && this.Bytes[this.End - 1] == 32)
				{
					this.End--;
				}
				if (this.Begin <= this.End - 2 && this.Bytes[this.End - 2] == 13 && this.Bytes[this.End - 1] == 10)
				{
					this.End -= 2;
				}
				while (this.Begin < this.End && this.Bytes[this.End - 1] == 32)
				{
					this.End--;
				}
			}
		}

		public void TrimSws()
		{
			this.TrimStartSws();
			this.TrimEndSws();
		}

		public void TrimStartComma()
		{
			if (this.Begin < this.End && this.Bytes[this.Begin] == 44)
			{
				this.Begin++;
			}
		}
	}
}
