using System;

namespace Boo.Lang.Compiler.TypeSystem
{
	public class NamespaceDelegator : INamespace
	{
		INamespace _parent;
		
		INamespace[] _namespaces;
		
		public NamespaceDelegator(INamespace parent, params INamespace[] namespaces)
		{
			if (null == namespaces)
			{
				throw new ArgumentNullException("namespaces");
			}
			_parent = parent;
			_namespaces = namespaces;
		}
		
		public INamespace ParentNamespace
		{
			get
			{
				return _parent;
			}
		}
		
		public bool Resolve(List targetList, string name, EntityType flags)
		{
			bool found = false;
			foreach (INamespace ns in _namespaces)
			{
				if (ns.Resolve(targetList, name, flags))
				{
					found = true;
				}
			}
			return found;
		}
		
		public IEntity[] GetMembers()
		{
			// TODO: implement this
			return NullNamespace.EmptyEntityArray;
		}
	}
}