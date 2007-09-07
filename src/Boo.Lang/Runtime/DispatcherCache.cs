using System.Collections.Generic;

namespace Boo.Lang.Runtime
{
	public class DispatcherCache
	{
		public delegate Dispatcher DispatcherFactory();

		private static Dictionary<DispatcherKey, Dispatcher> _cache =
			new Dictionary<DispatcherKey, Dispatcher>(DispatcherKey.EqualityComparer);

		/// <summary>
		/// Gets a dispatcher from the cache if available otherwise
		/// invokes factory to produce one and then cache it.
		/// </summary>
		/// <param name="key">the dispatcher key</param>
		/// <param name="factory">function to produce a dispatcher in case one it's not yet available</param>
		/// <returns></returns>
		public Dispatcher Get(DispatcherKey key, DispatcherFactory factory)
		{
			Dispatcher dispatcher;
			if (!_cache.TryGetValue(key, out dispatcher))
			{
				lock (_cache)
				{
					if (!_cache.TryGetValue(key, out dispatcher))
					{
						dispatcher = factory();
						_cache.Add(key, dispatcher);
					}
				}
			}
			return dispatcher;
		}

		/// <summary>
		/// Removes all Dispatchers from the cache.
		/// </summary>
		public void Clear()
		{
			lock (_cache)
			{
				_cache.Clear();
			}
		}
	}
}
