#if NETFX_CORE || NET_2_0 || NET_2_0_SUBSET
namespace System.Collections.Generic
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;

	public class SortedSet<T> : System.Collections.Generic.ICollection<T>, System.Collections.Generic.IEnumerable<T>, System.Collections.ICollection
	{
		private SortedDictionary<T, T> store;

		private class InitializationWrapper : IDictionary<T, T>
		{
			private readonly ICollection<T> data;

			public InitializationWrapper(IEnumerable<T> data)
			{
				this.data = data as ICollection<T>;
				if (this.data == null)
					this.data = data.ToList();
			}

			public T this[T key]
			{
				get
				{
					return key;
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			public ICollection<T> Keys
			{
				get
				{
					return data;
				}
			}

			public ICollection<T> Values
			{
				get
				{
					return data;
				}
			}

			public int Count
			{
				get
				{
					return data.Count;
				}
			}

			public bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public void Add(T key, T value)
			{
				throw new NotImplementedException();
			}

			public void Add(KeyValuePair<T, T> item)
			{
				throw new NotImplementedException();
			}

			public void Clear()
			{
				throw new NotImplementedException();
			}

			public bool Contains(KeyValuePair<T, T> item)
			{
				throw new NotImplementedException();
			}

			public bool ContainsKey(T key)
			{
				throw new NotImplementedException();
			}

			public void CopyTo(KeyValuePair<T, T>[] array, int arrayIndex)
			{
				throw new NotImplementedException();
			}

			public IEnumerator<KeyValuePair<T, T>> GetEnumerator()
			{
				foreach (T item in data)
					yield return new KeyValuePair<T, T>(item, item);
			}

			public bool Remove(T key)
			{
				throw new NotImplementedException();
			}

			public bool Remove(KeyValuePair<T, T> item)
			{
				throw new NotImplementedException();
			}

			public bool TryGetValue(T key, out T value)
			{
				throw new NotImplementedException();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				throw new NotImplementedException();
			}
		}

		public SortedSet()
		{
			store = new SortedDictionary<T, T>();
		}

		public SortedSet(IComparer<T> comparer)
		{
			store = new SortedDictionary<T, T>(comparer);
		}

		public SortedSet(IEnumerable<T> collection)
		{
			store = new SortedDictionary<T, T>(new InitializationWrapper(collection));
		}

		public SortedSet(IEnumerable<T> collection, IComparer<T> comparer)
		{
			store = new SortedDictionary<T, T>(new InitializationWrapper(collection), comparer);
		}

		public int Count
		{
			get
			{
				return store.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return ((ICollection)store).IsSynchronized;
			}
		}

		public object SyncRoot
		{
			get
			{
				return ((ICollection)store).SyncRoot;
			}
		}

		public void Add(T item)
		{
			store.Add(item, item);
		}

		public void Clear()
		{
			store.Clear();
		}

		public bool Contains(T item)
		{
			return store.ContainsKey(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			((ICollection)store).CopyTo(array, arrayIndex);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return store.Keys.GetEnumerator();
		}

		public bool Remove(T item)
		{
			return store.Remove(item);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return store.Keys.GetEnumerator();
		}

		public void CopyTo(Array array, int index)
		{
			((ICollection)store).CopyTo(array, index);
		}

		public void UnionWith(System.Collections.Generic.IEnumerable<T> other)
		{
			foreach (T item in other)
				Add(item);
		}
	}
}
#endif
