using System.Collections;
using System.Collections.Generic;

namespace Boo.Lang.Compiler.Util
{
	public class Set<T> : IEnumerable<T>, ICollection<T>
	{
		private readonly Dictionary<T, bool> _elements = new Dictionary<T, bool>();

		public void Add(T element)
		{
			_elements[element] = true;
		}

		public void Clear()
		{
			_elements.Clear();
		}

		public bool Contains(T element)
		{
			bool value;
			return _elements.TryGetValue(element, out value);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			throw new System.NotImplementedException();
		}

		public int Count
		{
			get { return _elements.Count; }
		}

		#region Implementation of IEnumerable
		public IEnumerator<T> GetEnumerator()
		{
			return _elements.Keys.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		#endregion

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(T element)
		{
			return _elements.Remove(element);
		}
	}
}
