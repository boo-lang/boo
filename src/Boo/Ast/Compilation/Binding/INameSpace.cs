using System;
using Boo.Ast;
using BindingFlags = System.Reflection.BindingFlags;

namespace Boo.Ast.Compilation.Binding
{	
	public interface INameSpace
	{		
		INameSpace Parent
		{
			get;
			set;
		}
		IBinding Resolve(string name);
	}
	
	class ModuleNameSpace : INameSpace
	{
		INameSpace _parent;
		
		Module _module;
		
		BindingManager _bindingManager;
		
		INameSpace[] _using;
		
		public ModuleNameSpace(BindingManager bindingManager, Module module)
		{
			_bindingManager = bindingManager;
			_module = module;
			_using = new INameSpace[_module.Using.Count];
			for (int i=0; i<_using.Length; ++i)
			{
				_using[i] = (INameSpace)bindingManager.GetBinding(_module.Using[i]);
			}
		}
		
		public INameSpace Parent
		{
			get
			{
				return _parent;
			}
			
			set
			{
				_parent = value;
			}
		}
		
		public IBinding Resolve(string name)
		{			
			foreach (TypeMember member in _module.Members)
			{
				if (name == member.Name)
				{
					return _bindingManager.GetBinding(member);
				}
			}
			
			foreach (INameSpace ns in _using)
			{
				// todo: resolve name in all namespaces...
				IBinding binding = ns.Resolve(name);
				if (null != binding)
				{
					return binding;
				}
			}
			
			if (null != _parent)
			{
				return _parent.Resolve(name);
			}
			return null;
		}
	}
}
