#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
//
// Contact Information
//
// mailto:rbo@acm.org
#endregion

using System;
using Boo.Lang.Ast;

namespace Boo.Lang.Compiler.Bindings
{
	public class ModuleNamespace : INamespace
	{
		BindingManager _bindingManager;
		
		Module _module;
		
		INamespace _moduleClassNamespace;
		
		INamespace[] _using;
		
		string _namespace;
		
		public ModuleNamespace(BindingManager bindingManager, Module module)
		{
			_bindingManager = bindingManager;
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
		
		public string Namespace
		{
			get
			{
				return _namespace;
			}
		}
		
		public IBinding ResolveMember(string name)
		{
			IBinding binding = ResolveModuleMember(name);
			if (null == binding)
			{
				binding = ResolveModuleClassMember(name);
			}
			return binding;
		}
		
		public IBinding Resolve(string name)
		{
			IBinding binding = ResolveMember(name);
			if (null == binding)
			{	
				if (null == _using)
				{
					_using = new INamespace[_module.Imports.Count];
					for (int i=0; i<_using.Length; ++i)
					{
						_using[i] = (INamespace)BindingManager.GetBinding(_module.Imports[i]);
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
		
		IBinding ResolveModuleMember(string name)
		{
			TypeMember member = _module.Members[name];
			if (null != member)
			{
				ITypeBinding typeBinding = (ITypeBinding)BindingManager.GetOptionalBinding(member);
				if (null == typeBinding)
				{
					if (NodeType.EnumDefinition == member.NodeType)
					{
						typeBinding = new EnumTypeBinding(_bindingManager, (EnumDefinition)member);
					}
					else
					{
						typeBinding = new InternalTypeBinding(_bindingManager, (TypeDefinition)member);
					}
					BindingManager.Bind(member, typeBinding);
				}
				return _bindingManager.AsTypeReference(typeBinding);
			}
			return null;
		}
		
		IBinding ResolveModuleClassMember(string name)
		{
			if (null == _moduleClassNamespace)
			{
				ClassDefinition moduleClass = Boo.Lang.Compiler.Pipeline.AstNormalizationStep.GetModuleClass(_module);
				if (null != moduleClass)
				{
					_moduleClassNamespace = (INamespace)ResolveModuleMember(moduleClass.Name);
				}
				
				if (null == _moduleClassNamespace)
				{
					_moduleClassNamespace = NullNamespace.Default;
				}
			}
			return _moduleClassNamespace.Resolve(name);
		}
	}
}
