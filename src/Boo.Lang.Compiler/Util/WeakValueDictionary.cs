using System;
using System.Collections.Generic;
using System.Diagnostics;

// based on code found at https://gist.github.com/qwertie/3867055
namespace Boo.Lang.Compiler.Util
{
	/// <summary>
	/// A dictionary in which the values are weak references. Written by DLP for SWIG.
	/// </summary>
	/// <remarks>
	/// Null values are not allowed in this dictionary.
	/// 
	/// When a value is garbage-collected, the dictionary acts as though the key is
	/// not present.
	/// 
	/// This class "cleans up" periodically by removing entries with garbage-collected
	/// values. Cleanups only occur occasionally, and only when the dictionary is accessed;
	/// Accessing it (for read or write) more often results in more frequent cleanups.
	///
	/// Watch out! The following interface members are not implemented:
	/// IDictionary.Values, ICollection.Contains, ICollection.CopyTo, ICollection.Remove.
	/// Also, the dictionary is NOT MULTITHREAD-SAFE.
	/// </remarks>
	public class WeakValueDictionary<TKey, TValue> : IDictionary<TKey, TValue>
		where TValue : class
	{
        private readonly Dictionary<TKey, WeakReference<TValue>> _dict = new();
        private int _version, _cleanVersion;
        private int _cleanGeneration;
        private const int MinRehashInterval = 500;

		public WeakValueDictionary()
		{
		}

		#region IDictionary<TKey,TValue> Members

		public ICollection<TKey> Keys => _dict.Keys;

		public ICollection<TValue> Values
		{   // TODO. Maybe. Eventually.
			get { throw new NotImplementedException(); }
		}

		public bool ContainsKey(TKey key)
		{
			AutoCleanup(1);

            if (!_dict.TryGetValue(key, out WeakReference<TValue> value))
                return false;
            return value.TryGetTarget(out _);
		}

		public void Add(TKey key, TValue value)
		{
			AutoCleanup(2);

            if (_dict.TryGetValue(key, out WeakReference<TValue> wr))
            {
                if (wr.TryGetTarget(out _))
                    throw new ArgumentException("An element with the same key already exists in this WeakValueDictionary");
                else
                    wr.SetTarget(value);
            }
            else
                _dict.Add(key, new WeakReference<TValue>(value));
        }

		public bool Remove(TKey key)
		{
			AutoCleanup(1);

            if (!_dict.TryGetValue(key, out WeakReference<TValue> wr))
                return false;
            _dict.Remove(key);
			return wr.TryGetTarget(out _);
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			AutoCleanup(1);

            if (_dict.TryGetValue(key, out WeakReference<TValue> wr))
                wr.TryGetTarget(out value);
            else
                value = null;
            return value != null;
		}

		public TValue this[TKey key]
		{
			get
			{
				var value = _dict[key];
				value.TryGetTarget(out var result);
				return result;
			}
			set
			{
				_dict[key] = new WeakReference<TValue>(value);
			}
		}

		void AutoCleanup(int incVersion)
		{
			_version += incVersion;

			// Cleanup the table every so often--less often for larger tables.
			long delta = _version - _cleanVersion;
			if (delta > MinRehashInterval + _dict.Count)
			{
				// A cleanup will be fruitless unless a GC has happened in the meantime.
				// WeakReferences can become zero only during the GC.
				int curGeneration = GC.CollectionCount(0);
				if (_cleanGeneration != curGeneration)
				{
					_cleanGeneration = curGeneration;
					Cleanup();
					_cleanVersion = _version;
				}
				else
					_cleanVersion += MinRehashInterval; // Wait a little while longer
			}
		}
		void Cleanup()
		{
			// Remove all pairs whose value is nullified.
			// Due to the fact that you can't change a Dictionary while enumerating 
			// it, we need an intermediate collection (the list of things to delete):
			var deadKeys = new List<TKey>();

			foreach (KeyValuePair<TKey, WeakReference<TValue>> kvp in _dict)
				if (!kvp.Value.TryGetTarget(out _))
					deadKeys.Add(kvp.Key);

			foreach (TKey key in deadKeys)
			{
				bool success = _dict.Remove(key);
				Debug.Assert(success);
			}
		}
		#endregion

		#region ICollection<KeyValuePair<TKey,TValue>> Members

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			Add(item.Key, item.Value);
		}
		public void Clear()
		{
			_dict.Clear();
			_version = _cleanVersion = 0;
			_cleanGeneration = 0;
		}
		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			throw new Exception("The method or operation is not implemented.");
		}
		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			throw new Exception("The method or operation is not implemented.");
		}
		public int Count
		{
			// THIS VALUE MAY BE WRONG (i.e. it may be higher than the number of 
			// items you get from the iterator).
			get { return _dict.Count; }
		}
		public bool IsReadOnly
		{
			get { return false; }
		}
		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		#region IEnumerable<KeyValuePair<TKey,TValue>> Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			int nullCount = 0;

			foreach (KeyValuePair<TKey, WeakReference<TValue>> kvp in _dict)
			{
				var value = kvp.Value;
				value.TryGetTarget(out var target);
				if (target == null)
					nullCount++;
				else
					yield return new KeyValuePair<TKey, TValue>(kvp.Key, target);
			}

			if (nullCount > _dict.Count / 4)
				Cleanup();
		}

		#endregion
	}
}
