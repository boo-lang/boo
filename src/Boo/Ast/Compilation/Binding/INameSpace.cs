using System;
using System.Collections;
using Boo.Ast;
using BindingFlags = System.Reflection.BindingFlags;

namespace Boo.Ast.Compilation.Binding
{	
	public interface INamespace
	{				
		IBinding Resolve(string name);
	}
	
	class DeclarationsNamespace : INamespace
	{
		BindingManager _bindingManager;
		DeclarationCollection _declarations;
		
		public DeclarationsNamespace(BindingManager bindingManager, DeclarationCollection declarations)
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
}
