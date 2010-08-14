using System;
using System.Collections;
using System.Collections.Generic;

namespace Boo.Lang.Environments
{
	public class DeferredEnvironment : IEnumerable<KeyValuePair<Type, Func<object>>>, IEnvironment
	{
		private readonly List<KeyValuePair<Type, Func<object>>> _bindings = new List<KeyValuePair<Type, Func<object>>>();

		public void Add(Type need, Func<object> binder)
		{
			_bindings.Add(new KeyValuePair<Type, Func<object>>(need, binder));
		}

		IEnumerator<KeyValuePair<Type, Func<object>>> IEnumerable<KeyValuePair<Type, Func<object>>>.GetEnumerator()
		{
			return _bindings.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _bindings.GetEnumerator();
		}

		TNeed IEnvironment.Provide<TNeed>()
		{
			foreach (var binding in _bindings)
				if (typeof(TNeed).IsAssignableFrom(binding.Key))
					return (TNeed) binding.Value();
			return null;
		}
	}
}
