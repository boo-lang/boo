using System;
using System.Collections.Generic;

namespace Boo.Lang.Environments
{
	public class CachingEnvironment : IEnvironment
	{
		private readonly Dictionary<Type, object> _cache = new Dictionary<Type,object>();
		private readonly IEnvironment _source;

		public event Action<object> InstanceCached;

		public CachingEnvironment(IEnvironment source)
		{
			_source = source;
		}

		public TNeed Provide<TNeed>() where TNeed : class
		{
			object cached;
			if (_cache.TryGetValue(typeof(TNeed), out cached))
				return (TNeed) cached;

			foreach (var instance in _cache.Values)
				if (instance is TNeed)
				{
					_cache.Add(typeof(TNeed), instance);
					return (TNeed) instance;
				}

			var newInstance = _source.Provide<TNeed>();
			if (newInstance != null)
				Add(typeof(TNeed), newInstance);
			return newInstance;
		}

		public void Add(Type type, object instance)
		{
			if (!type.IsInstanceOfType(instance))
				throw new ArgumentException(string.Format("{0} is not an instance of {1}", instance, type));

			_cache.Add(type, instance);

			if (null != InstanceCached)
				InstanceCached(instance);
		}
	}
}
