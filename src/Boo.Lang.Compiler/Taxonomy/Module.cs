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

namespace Boo.Lang.Compiler.Taxonomy
{
	using System;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.Services;

	public class ModuleInfo : INamespace, IInfo
	{
		TaxonomyManager _bindingService;
		
		Module _module;
		
		INamespace _moduleClassNamespace = NullNamespace.Default;
		
		INamespace[] _using;
		
		string _namespace;
		
		public ModuleInfo(TaxonomyManager bindingManager, Module module)
		{
			_bindingService = bindingManager;
			_module = module;			
			if (null == module.Namespace)
			{
				_namespace = "";
			}
			else
			{
				_namespace = module.Namespace.Name;
			}
		}
		
		public InfoType InfoType
		{
			get
			{
				return InfoType.Module;
			}
		}
		
		public string Name
		{
			get
			{
				return _module.Name;
			}
		}
		
		public string FullName
		{
			get
			{
				return _module.FullName;
			}
		}
		
		public string Namespace
		{
			get
			{
				return _namespace;
			}
		}
		
		public IInfo ResolveMember(string name)
		{
			IInfo binding = ResolveModuleMember(name);
			if (null == binding)
			{
				binding = ResolveModuleClassMember(name);
			}
			return binding;
		}
		
		public INamespace ParentNamespace
		{
			get
			{
				return (INamespace)TaxonomyManager.GetInfo(_module.ParentNode);
			}
		}
		
		public IInfo Resolve(string name)
		{
			IInfo binding = ResolveMember(name);
			if (null == binding)
			{	
				if (null == _using)
				{
					_using = new INamespace[_module.Imports.Count];
					for (int i=0; i<_using.Length; ++i)
					{
						_using[i] = (INamespace)TaxonomyManager.GetInfo(_module.Imports[i]);
					}
				}
				
				foreach (INamespace ns in _using)
				{
					// todo: resolve name in all namespaces...
					binding = ns.Resolve(name);
					if (null != binding)
					{					
						break;
					}
				}
			}
			return binding;
		}
		
		IInfo ResolveModuleMember(string name)
		{
			TypeMember member = _module.Members[name];
			if (null != member)
			{
				ITypeInfo typeInfo = (ITypeInfo)member.Info;
				if (null == typeInfo)
				{
					if (NodeType.EnumDefinition == member.NodeType)
					{
						typeInfo = new EnumTypeInfo(_bindingService, (EnumDefinition)member);
					}
					else
					{
						typeInfo = new InternalType(_bindingService, (TypeDefinition)member);
					}
					TaxonomyManager.Bind(member, typeInfo);
				}
				return _bindingService.AsTypeReference(typeInfo);
			}
			return null;
		}
		
		IInfo ResolveModuleClassMember(string name)
		{
			return _moduleClassNamespace.Resolve(name);
		}
	}
}
