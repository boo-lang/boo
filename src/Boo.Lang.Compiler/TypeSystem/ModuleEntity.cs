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

namespace Boo.Lang.Compiler.TypeSystem
{
	using System;

	public class ModuleEntity : INamespace, IEntity
	{
		NameResolutionService _nameResolutionService;
		
		TypeSystemServices _typeSystemServices;
		
		Boo.Lang.Compiler.Ast.Module _module;
		
		INamespace _moduleClassNamespace = NullNamespace.Default;
		
		INamespace[] _using;
		
		string _namespace;
		
		public ModuleEntity(NameResolutionService nameResolutionService, TypeSystemServices tagManager, Boo.Lang.Compiler.Ast.Module module)
		{
			_nameResolutionService = nameResolutionService;
			_typeSystemServices = tagManager;
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
		
		public EntityType EntityType
		{
			get
			{
				return EntityType.Module;
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
		
		public void InitializeModuleClass(Boo.Lang.Compiler.Ast.ClassDefinition moduleClass)
		{
			if (null == moduleClass.Entity)
			{
				moduleClass.Entity = new InternalType(_typeSystemServices, moduleClass);
			}
			_moduleClassNamespace = (INamespace)moduleClass.Entity;
		}
		
		public bool ResolveMember(Boo.Lang.List targetList, string name, EntityType flags)
		{
			if (ResolveModuleMember(targetList, name, flags))
			{
				return true;
			}
			return _moduleClassNamespace.Resolve(targetList, name, flags);
		}
		
		public INamespace ParentNamespace
		{
			get
			{
				return _nameResolutionService.GlobalNamespace;
			}
		}
		
		public bool Resolve(Boo.Lang.List targetList, string name, EntityType flags)
		{
			if (ResolveMember(targetList, name, flags))
			{
				return true;
			}
			
			if (null == _using)
			{
				_using = new INamespace[_module.Imports.Count];
				for (int i=0; i<_using.Length; ++i)
				{
					_using[i] = (INamespace)TypeSystemServices.GetEntity(_module.Imports[i]);
				}
			}
				
			bool found = false;
			foreach (INamespace ns in _using)
			{			
				if (ns.Resolve(targetList, name, flags))
				{
					found = true;
				}
			}
			return found;
		}
		
		bool ResolveModuleMember(Boo.Lang.List targetList, string name, EntityType flags)
		{
			bool found=false;
			foreach (Boo.Lang.Compiler.Ast.TypeMember member in _module.Members)
			{
				if (name == member.Name)
				{
					IEntity tag = TypeSystemServices.GetEntity(member);
					if (NameResolutionService.IsFlagSet(flags, tag.EntityType))
					{
						targetList.Add(tag);
						found = true;
					}
				}
			}
			return found;
		}
	}
}
