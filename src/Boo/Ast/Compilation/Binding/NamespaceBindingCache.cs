using System;
using System.Collections;

namespace Boo.Ast.Compilation.Binding
{
	public class NamespaceBindingCache
	{
		protected Hashtable _bindingCache = new Hashtable();
		
		public IBinding ResolveFromCache(string name, out bool found)
		{
			IBinding binding = (IBinding)_bindingCache[name];
			if (null == binding)
			{
				found = _bindingCache.ContainsKey(name);
			}
			else
			{
				found = true;
			}
			return binding;
		}
		
		public IBinding Cache(string name, IBinding binding)
		{
			_bindingCache[name] = binding;
			return binding;
		}
	}
}
