#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, 
// publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

namespace Boo.Lang.Compiler.Bindings
{
	using System;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Steps;
	
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
