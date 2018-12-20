using System;
using System.Collections.Generic;
using System.Threading;

namespace SocketServers
{
	public class ThreadSafeDictionary<K, T> where T : class
	{
		private ReaderWriterLockSlim sync;

		private Dictionary<K, T> dictionary;

		public ThreadSafeDictionary() : this(-1, null)
		{
		}

		public ThreadSafeDictionary(int capacity) : this(capacity, null)
		{
		}

		public ThreadSafeDictionary(int capacity, IEqualityComparer<K> comparer)
		{
			this.sync = new ReaderWriterLockSlim();
			if (capacity > 0)
			{
				this.dictionary = new Dictionary<K, T>(capacity, comparer);
				return;
			}
			this.dictionary = new Dictionary<K, T>(comparer);
		}

		public void Clear()
		{
			try
			{
				this.sync.EnterWriteLock();
				this.dictionary.Clear();
			}
			finally
			{
				this.sync.ExitWriteLock();
			}
		}

		public void Add(K key, T value)
		{
			try
			{
				this.sync.EnterWriteLock();
				this.dictionary.Add(key, value);
			}
			finally
			{
				this.sync.ExitWriteLock();
			}
		}

		public bool TryAdd(K key, T value)
		{
			bool result;
			try
			{
				this.sync.EnterWriteLock();
				if (this.dictionary.ContainsKey(key))
				{
					result = false;
				}
				else
				{
					this.dictionary.Add(key, value);
					result = true;
				}
			}
			finally
			{
				this.sync.ExitWriteLock();
			}
			return result;
		}

		public T Replace(K key, T value)
		{
			T result;
			try
			{
				this.sync.EnterWriteLock();
				T t;
				if (this.dictionary.TryGetValue(key, out t))
				{
					this.dictionary.Remove(key);
				}
				this.dictionary.Add(key, value);
				result = t;
			}
			finally
			{
				this.sync.ExitWriteLock();
			}
			return result;
		}

		public bool Remove(K key, T value)
		{
			bool result = false;
			try
			{
				this.sync.EnterWriteLock();
				T t;
				if (this.dictionary.TryGetValue(key, out t) && t == value)
				{
					result = this.dictionary.Remove(key);
				}
			}
			finally
			{
				this.sync.ExitWriteLock();
			}
			return result;
		}

		public void ForEach(Action<T> action)
		{
			try
			{
				this.sync.EnterReadLock();
				foreach (KeyValuePair<K, T> current in this.dictionary)
				{
					action(current.Value);
				}
			}
			finally
			{
				this.sync.ExitReadLock();
			}
		}

		public bool Contain(Func<T, bool> predicate)
		{
			try
			{
				this.sync.EnterReadLock();
				foreach (KeyValuePair<K, T> current in this.dictionary)
				{
					if (predicate(current.Value))
					{
						return true;
					}
				}
			}
			finally
			{
				this.sync.ExitReadLock();
			}
			return false;
		}

		public void Remove(Predicate<K> match, Action<T> removed)
		{
			try
			{
				this.sync.EnterWriteLock();
				List<K> list = new List<K>();
				foreach (KeyValuePair<K, T> current in this.dictionary)
				{
					if (match(current.Key))
					{
						removed(current.Value);
						list.Add(current.Key);
					}
				}
				foreach (K current2 in list)
				{
					this.dictionary.Remove(current2);
				}
			}
			finally
			{
				this.sync.ExitWriteLock();
			}
		}

		public bool TryGetValue(K key, out T value)
		{
			bool result;
			try
			{
				this.sync.EnterReadLock();
				result = this.dictionary.TryGetValue(key, out value);
			}
			finally
			{
				this.sync.ExitReadLock();
			}
			return result;
		}

		public T GetValue(K key)
		{
			T result;
			try
			{
				this.sync.EnterReadLock();
				this.dictionary.TryGetValue(key, out result);
			}
			finally
			{
				this.sync.ExitReadLock();
			}
			return result;
		}

		public bool ContainsKey(K key)
		{
			bool result;
			try
			{
				this.sync.EnterReadLock();
				result = this.dictionary.ContainsKey(key);
			}
			finally
			{
				this.sync.ExitReadLock();
			}
			return result;
		}
	}
}
