using System;
using Boo.Ast;
using BindingFlags = System.Reflection.BindingFlags;

namespace Boo.Ast.Compilation.Binding
{	
	public interface INameSpace
	{				
		IBinding Resolve(string name);
	}
	
	class DeclarationsNameSpace : INameSpace
	{
		BindingManager _bindingManager;
		DeclarationCollection _declarations;
		
		public DeclarationsNameSpace(BindingManager bindingManager, DeclarationCollection declarations)
		{
			_bindingManager = bindingManager;
			_declarations = declarations;
		}
		
		public IBinding Resolve(string name)
		{
			Declaration d = _declarations[name];
			if (null != d)
			{
				return _bindingManager.GetBinding(d);
			}
			return null;
		}
	}
	
	class ModuleNameSpace : INameSpace
	{
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
			return null;
		}
	}
}
