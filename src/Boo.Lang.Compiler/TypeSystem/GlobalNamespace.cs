using System.Collections;

namespace Boo.Lang.Compiler.TypeSystem
{
	public class GlobalNamespace : SimpleNamespace
	{
		INamespace _empty;
		
		public GlobalNamespace() : this(new Hashtable())
		{
		}
		
		public GlobalNamespace(IDictionary children) : base(null, children)
		{
			_empty = (INamespace)children[""];
			if (null == _empty)
			{
				_empty = NullNamespace.Default;
			}
		}
		
		override public bool Resolve(List targetList, string name, EntityType flags)
		{
			if (!base.Resolve(targetList, name, flags))
			{
				return _empty.Resolve(targetList, name, flags);
			}
			return true;
		}
		
		public INamespace GetChild(string name)
		{
			return (INamespace)_children[name];
		}
		
		public void SetChild(string name, INamespace entity)
		{
			_children[name] = entity;
			if (name == "")
			{
				_empty = entity;
			}
		}
	}
}