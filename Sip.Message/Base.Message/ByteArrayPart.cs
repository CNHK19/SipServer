using System;
using System.Net;
using System.Text;

namespace Base.Message
{
	public struct ByteArrayPart : IEquatable<ByteArrayPart>
	{
		private const byte Cl = 10;

		private const byte Cr = 13;

		private const byte Space = 32;

		private const byte Comma = 44;

		public byte[] Bytes;

		public int Begin;

		public int End;

		public static readonly ByteArrayPart Invalid = new ByteArrayPart
		{
			Bytes = null,
			Begin = -2147483648,
			End = -2147483648
		};

		public bool IsValid
		{
			get
			{
				return this.Begin >= 0 && this.End >= 0;
			}
		}

		public bool IsNotEmpty
		{
			get
			{
				return this.Begin >= 0 && this.End >= 0 && this.Begin < this.End;
			}
		}

		public bool IsInvalid
		{
			get
			{
				return this.Begin < 0 || this.End < 0;
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
				return this.End - this.Begin;
			}
		}

		public ByteArrayPart(byte[] array, int offset, int length)
		{
			this.Bytes = new byte[length];
			this.Begin = 0;
			this.End = length;
			Buffer.BlockCopy(array, offset, this.Bytes, 0, length);
		}

		public ByteArrayPart(ByteArrayPart part)
		{
			this = new ByteArrayPart(part.Bytes, part.Offset, part.Length);
		}

		public ByteArrayPart(string text)
		{
			this.Bytes = Encoding.UTF8.GetBytes(text);
			this.Begin = 0;
			this.End = this.Bytes.Length;
		}

		public ByteArrayPart(char simbol)
		{
			this.Bytes = Encoding.UTF8.GetBytes(new char[]
			{
				simbol
			});
			this.Begin = 0;
			this.End = this.Bytes.Length;
		}

		public bool IsEqualValue(ByteArrayPart y)
		{
			if (this.Length != y.Length)
			{
				return false;
			}
			for (int i = 0; i < this.Length; i++)
			{
				if (this.Bytes[this.Begin + i] != y.Bytes[y.Begin + i])
				{
					return false;
				}
			}
			return true;
		}

		public bool Equals(byte[] bytes)
		{
			int length = this.Length;
			if (length != bytes.Length)
			{
				return false;
			}
			for (int i = 0; i < length; i++)
			{
				if (this.Bytes[this.Begin + i] != bytes[i])
				{
					return false;
				}
			}
			return true;
		}

		public bool Equals(ByteArrayPart other)
		{
			return this.IsEqualValue(other);
		}

		public static bool operator ==(ByteArrayPart x, ByteArrayPart y)
		{
			return x.IsEqualValue(y);
		}

		public static bool operator !=(ByteArrayPart x, ByteArrayPart y)
		{
			return !x.IsEqualValue(y);
		}

		public override bool Equals(object obj)
		{
			return obj is ByteArrayPart && this.IsEqualValue((ByteArrayPart)obj);
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

		public void SetDefaultValue()
		{
			this.Bytes = null;
			this.Begin = -2147483648;
			this.End = -2147483648;
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

		public IPAddress ToIpAddress()
		{
			IPAddress none = IPAddress.None;
			if (this.IsValid)
			{
				IPAddress.TryParse(this.ToString(), out none);
			}
			return none;
		}

		public override string ToString()
		{
			if (this.Bytes == null || this.Begin < 0 || this.End < 0)
			{
				return null;
			}
			return Encoding.UTF8.GetString(this.Bytes, this.Offset, this.Length);
		}

		public string ToString(int startIndex)
		{
			if (this.Bytes == null || this.Begin < 0 || this.End < 0)
			{
				return null;
			}
			return Encoding.UTF8.GetString(this.Bytes, this.Offset + startIndex, this.Length - startIndex);
		}

		public ArraySegment<byte> ToArraySegment()
		{
			return new ArraySegment<byte>(this.Bytes, this.Offset, this.Length);
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
	}
}
