using System;
using System.Collections;

namespace Boo.Lang.Compiler.TypeSystem
{
	public class SimpleNamespace : INamespace
	{		
		protected INamespace _parent;
		protected IDictionary _children;
		
		public SimpleNamespace(INamespace parent, IDictionary children)
		{
			if (null == children)
			{
				throw new ArgumentNullException("children");
			}
			_parent = parent;
			_children = children;			
		}
		
		public INamespace ParentNamespace
		{
			get
			{
				return _parent;
			}
		}
		
		public virtual bool Resolve(List targetList, string name, EntityType flags)
		{
			IEntity element = (IEntity)_children[name];
			if (null != element && NameResolutionService.IsFlagSet(flags, element.EntityType))
			{
				targetList.Add(element);
				return true;
			}
			return false;
		}
		
		public virtual IEntity[] GetMembers()
		{
			return (IEntity[])Builtins.array(typeof(IEntity), _children.Values);
		}
	}
}