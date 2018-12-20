using System;
using System.Collections.Generic;

namespace Base.Message
{
	public sealed class ByteArrayPartEqualityComparer : IEqualityComparer<ByteArrayPart>
	{
		private static readonly ByteArrayPartEqualityComparer instance = new ByteArrayPartEqualityComparer();

		private ByteArrayPartEqualityComparer()
		{
		}

		public static IEqualityComparer<ByteArrayPart> GetStaticInstance()
		{
			return ByteArrayPartEqualityComparer.instance;
		}

		public bool Equals(ByteArrayPart x, ByteArrayPart y)
		{
			return x.IsEqualValue(y);
		}

		public int GetHashCode(ByteArrayPart x)
		{
			return x.GetHashCode();
		}
	}
}
