#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
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
