namespace Boo.Lang.Compiler.Bindings
{
	using System;
	using Boo.Lang.Ast;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Pipeline;
	
	public class CompileUnitBinding : IBinding, INamespace
	{
		INamespace _parent;
		
		INamespace[] _namespaces;
		
		public CompileUnitBinding(INamespace parent)
		{
			// Global names at the highest level
			_parent = parent;
			
			INamespace boolang = (INamespace)((INamespace)_parent.Resolve("Boo")).Resolve("Lang");
			INamespace builtins = (INamespace)boolang.Resolve("Builtins");
			
			// namespaces that are resolved as 'this' namespace
			// in order of preference
			_namespaces = new INamespace[2];
			_namespaces[0] = builtins;
			_namespaces[1] = boolang;
		}
		
		public BindingType BindingType
		{
			get
			{
				return BindingType.CompileUnit;
			}
		}
		
		public string Name
		{
			get
			{
				return "Global";
			}
		}
		
		public string FullName
		{
			get
			{
				return "Global";
			}
		}
		
		public INamespace ParentNamespace
		{
			get
			{
				return _parent;
			}
		}
		
		public IBinding Resolve(string name)
		{
			foreach (INamespace ns in _namespaces)
			{
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
