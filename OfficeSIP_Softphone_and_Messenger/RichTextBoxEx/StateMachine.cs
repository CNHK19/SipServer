// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Collections.Generic;

namespace RichTextBoxEx
{
	class StateMachine
	{
		private const int minCharCode = 32;
		private const int maxCharCode = 126;

		private struct State
		{
			private ushort state;

			public int NextState
			{
				get
				{
					return state & 0x00ff;
				}
				set
				{
					state &= 0xff00;
					state |= (ushort)(value & 0x00ff);
				}
			}

			public int Id
			{
				get
				{
					return ((state & 0xff00) >> 8) - 1;
				}
				set
				{
					state &= 0x00ff;
					state |= (ushort)(((value & 0x00ff) + 1) << 8);
				}
			}

			public bool IsEndState()
			{
				return (state & 0xff00) != 0;
			}

			public bool IsEmpty()
			{
				return state == 0;
			}

			public bool IsNotEndState()
			{
				return !IsEndState();
			}

			public bool IsNotEmpty()
			{
				return !IsEmpty();
			}

			public bool IsInitialState()
			{
				return state == 0;
			}

			public void Empty()
			{
				state = 0;
			}
		}

		private class StateArray
		{
			private State[] states = new State[maxCharCode - minCharCode + 1];

			public State this[int i]
			{
				get
				{
					return states[i - minCharCode];
				}
				set
				{
					states[i - minCharCode] = value;
				}
			}

			public void SetNextState(char i, int nextState)
			{
				states[i - minCharCode].NextState = nextState;
			}

			public void SetId(char i, int id)
			{
				states[i - minCharCode].Id = id;
			}
		}

		private List<StateArray> states;
		private State state;
		private int step;
	//	private bool found;

		public StateMachine()
		{
			state = new State();
			step = 0;
			MaxLength = 0;

			states = new List<StateArray>(32);
			states.Add(new StateArray());
		}

		private bool IsValidChar(char c)
		{
			return c >= minCharCode && c <= maxCharCode;
		}

		public int StatesCount
		{
			get
			{
				return states.Count;
			}
		}

		public int MaxLength
		{
			get;
			private set;
		}

		public bool Add(string substring, int id)
		{
			foreach (char c in substring)
				if (IsValidChar(c) == false)
					return false;

			if (MaxLength < substring.Length)
				MaxLength = substring.Length;

			int current = 0;

			for (int i = 0; ; i++)
			{
				char c = substring[i];

				if (i >= substring.Length - 1)
				{
					states[current].SetId(c, id);
					break;
				}
				else
				{
					if (states[current][c].NextState == 0)
					{
						states.Add(new StateArray());
						states[current].SetNextState(c, states.Count - 1);
					}

					current = states[current][c].NextState;
				}
			}

			return true;
		}

		public bool Step(char simbol, out bool start, out bool end, ref int id)
		{
			start = end = false;

			var oldState = state;

			if (IsValidChar(simbol))
			{
				state = states[state.NextState][simbol];

				start = (oldState.IsEmpty() && state.IsNotEmpty());

				if (state.IsEndState())
				{
					end = true;
					id = state.Id;
				}
			}
			else
				state.Empty();

			return oldState.IsNotEmpty() && state.NextState == 0;
		}

		public int GetId(string simbols)
		{
			State tempState = new State();

			foreach (char c in simbols)
			{
				if (IsValidChar(c) == false)
					return -1;

				tempState = states[tempState.NextState][c];

				if (tempState.IsEmpty())
					return -1;
			}

			return tempState.Id;
		}

		public bool End()
		{
			var oldState = state;
			state.Empty();

			return oldState.IsEndState();
		}


		public void Reset()
		{
			state.Empty();
			step = 0;
		}

		public bool Step(char simbol, out bool saveStart)
		{
			State oldState = state;

			int endState, length;
			bool found = Step(simbol, out endState, out length);

			saveStart = (oldState.IsInitialState() && !state.IsInitialState());

			return found;
		}

		public bool Step(char simbol, out int endState, out int length)
		{
			if (IsValidChar(simbol) == false)
			{
				Reset();

				endState = 0;
				length = 0;
				return false;
			}

			state = states[state.NextState][simbol];
			step++;

			endState = state.Id;
			length = step;
			bool found = state.IsEndState();

			if (state.IsInitialState() || state.IsEndState())
				Reset();

			return found;
		}

		public bool Steps(string simbols, out int endState, out int length)
		{
			endState = -1;
			length = -1;

			foreach (char c in simbols)
			{
				if (Step(c, out endState, out length))
					return true;
			}

			return false;
		}
	}
}
