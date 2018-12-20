// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RichTextBoxEx
{
	class RabinKarp<T, U>
		where T : class
		where U : class
	{
		struct Substring
		{
			public Substring(string value, U data)
			{
				Value = value;
				Data = data;
			}

			public string Value;
			public U Data;
		}

		private const int size = 4;
		private readonly Dictionary<uint, Substring> substrings = new Dictionary<uint, Substring>();
		private readonly T[] starts = new T[size];
		private readonly T[] ends = new T[size];
		private readonly char[] simbols = new char[size];
		private int pointer = 0;
		private uint key;

		public int EndSteps
		{
			get
			{
				return size - 2;
			}
		}

		public int MaxLength
		{
			get
			{
				return size;
			}
		}

		public void Add(string subsctring, U data)
		{
			if (subsctring.Length >= 2 && subsctring.Length <= size)
				substrings.Add(GetKey(subsctring), new Substring(subsctring, data));
		}

		public U Get(string pattern)
		{
			if (pattern.Length >= 2 && pattern.Length <= size)
			{
				Substring substring;
				if (substrings.TryGetValue(GetKey(pattern), out substring))
					return substring.Data;
			}

			return null;
		}

		public void Reset()
		{
			key = 0;

			for (int i = 1; i < starts.Length; i++)
			{
				starts[i - 1] = starts[i];
				ends[i - 1] = ends[i];
			}
		}

		public bool Step(char simbol, T simbolStart, T simbolEnd, out U data, out T start, out T end)
		{
			key <<= 8;
			key |= (uint)simbol & 0xff;

			starts[pointer] = simbolStart;
			ends[pointer] = simbolEnd;
			simbols[pointer] = simbol;

			pointer = (pointer + 1) % size;

			data = null;
			start = end = null;

			for (int i = 0; i < 3; i++)
			{
				Substring substring;
				if (substrings.TryGetValue(key & (0xffffffff << i * 8), out substring) && Compare(substring.Value))
				{
					key &= ~(0xffffffff << i * 8);

					data = substring.Data;
					start = starts[pointer % size];
					end = ends[(pointer + size - i - 1) % size];

					break;
				}
			}

			return data != null;
		}

		private bool Compare(string substring)
		{
			for (int i = 0; i < Math.Min(size, substring.Length); i++)
				if (simbols[(pointer + i) % size] != substring[i])
					return false;

			return true;
		}

		public bool Step(out U data, out T start, out T end)
		{
			return Step(' ', null, null, out data, out start, out end);
		}

		private static uint GetKey(string substring)
		{
			uint key = 0;

			for (int i = 0; i < size; i++)
			{
				key <<= 8;
				if (i < substring.Length)
					key |= substring[i];
			}

			return key;
		}
	}
}
