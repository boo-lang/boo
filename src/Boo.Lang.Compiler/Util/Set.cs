using System.Collections;
using System.Collections.Generic;

namespace Boo.Lang.Compiler.Util
{
	public class Set<T> : IEnumerable<T>
	{
		private readonly Dictionary<T, bool> _elements = new Dictionary<T, bool>();

		public void Add(T element)
		{
			_elements[element] = true;
		}

		public bool Contains(T element)
		{
			bool value;
			return _elements.TryGetValue(element, out value);
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

		public void Clear()
		{
			_elements.Clear();
		}
	}
}
