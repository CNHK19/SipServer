using System;

namespace Base.Message
{
	public static class Base64Encoding
	{
		private static readonly byte[] encodingTable = new byte[]
		{
			65,
			66,
			67,
			68,
			69,
			70,
			71,
			72,
			73,
			74,
			75,
			76,
			77,
			78,
			79,
			80,
			81,
			82,
			83,
			84,
			85,
			86,
			87,
			88,
			89,
			90,
			97,
			98,
			99,
			100,
			101,
			102,
			103,
			104,
			105,
			106,
			107,
			108,
			109,
			110,
			111,
			112,
			113,
			114,
			115,
			116,
			117,
			118,
			119,
			120,
			121,
			122,
			48,
			49,
			50,
			51,
			52,
			53,
			54,
			55,
			56,
			57,
			43,
			47
		};

		private static readonly byte padding = 61;

		public static int GetEncodedLength(int length)
		{
			int num = length % 3;
			int num2 = length - num;
			return num2 / 3 * 4 + ((num == 0) ? 0 : 4);
		}

		public static int Encode(ArraySegment<byte> segment, byte[] output, int outputOffset)
		{
			return Base64Encoding.Encode(segment.Array, segment.Offset, segment.Count, output, outputOffset);
		}

		public static int Encode(byte[] input, int inputOffset, int length, byte[] output, int outputOffset)
		{
			int num = length % 3;
			int num2 = length - num;
			for (int i = inputOffset; i < inputOffset + num2; i += 3)
			{
				int num3 = (int)(input[i] & 255);
				int num4 = (int)(input[i + 1] & 255);
				int num5 = (int)(input[i + 2] & 255);
				output[outputOffset++] = Base64Encoding.encodingTable[(int)((uint)num3 >> 2 & 63u)];
				output[outputOffset++] = Base64Encoding.encodingTable[(num3 << 4 | (int)((uint)num4 >> 4)) & 63];
				output[outputOffset++] = Base64Encoding.encodingTable[(num4 << 2 | (int)((uint)num5 >> 6)) & 63];
				output[outputOffset++] = Base64Encoding.encodingTable[num5 & 63];
			}
			switch (num)
			{
			case 1:
			{
				int num6 = (int)(input[inputOffset + num2] & 255);
				int num7 = num6 >> 2 & 63;
				int num8 = num6 << 4 & 63;
				output[outputOffset++] = Base64Encoding.encodingTable[num7];
				output[outputOffset++] = Base64Encoding.encodingTable[num8];
				output[outputOffset++] = Base64Encoding.padding;
				output[outputOffset++] = Base64Encoding.padding;
				break;
			}
			case 2:
			{
				int num6 = (int)(input[inputOffset + num2] & 255);
				int num9 = (int)(input[inputOffset + num2 + 1] & 255);
				int num7 = num6 >> 2 & 63;
				int num8 = (num6 << 4 | num9 >> 4) & 63;
				int num10 = num9 << 2 & 63;
				output[outputOffset++] = Base64Encoding.encodingTable[num7];
				output[outputOffset++] = Base64Encoding.encodingTable[num8];
				output[outputOffset++] = Base64Encoding.encodingTable[num10];
				output[outputOffset++] = Base64Encoding.padding;
				break;
			}
			}
			return num2 / 3 * 4 + ((num == 0) ? 0 : 4);
		}
	}
}
