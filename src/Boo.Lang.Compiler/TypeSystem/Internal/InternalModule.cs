#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem.Core;
using Boo.Lang.Compiler.TypeSystem.Services;
using Boo.Lang.Compiler.Util;

namespace Boo.Lang.Compiler.TypeSystem.Internal
{
	public class InternalModule : INamespace
	{
		private readonly InternalTypeSystemProvider _provider;
		
		private readonly Module _module;
		
		private ClassDefinition _moduleClass;
		
		private INamespace _moduleClassNamespace = NullNamespace.Default;

		private INamespace _moduleAsNamespace;
		
		private string _namespace;
		
		public InternalModule(InternalTypeSystemProvider provider, Module module)
		{
			_provider = provider;
			_module = module;			
			_namespace = SafeNamespace(module);
		}

		public static string SafeNamespace(Module module)
		{
			return null == module.Namespace ? string.Empty : module.Namespace.Name;
		}

		public EntityType EntityType
		{
			get { return EntityType.Namespace; }
		}
		
		public string Name
		{
			get { return _module.Name; }
		}
		
		public string FullName
		{
			get { return _module.FullName; }
		}
		
		public string Namespace
		{
			get { return _namespace; }
		}
		
		public Boo.Lang.Compiler.Ast.ClassDefinition ModuleClass
		{
			get { return _moduleClass; }
		}

		public INamespace ModuleMembersNamespace
		{
			get
			{
				if (_moduleAsNamespace != null)
					return _moduleAsNamespace;

				return _moduleAsNamespace = new ModuleMembersNamespace(this);
			}
		}
		
		public void InitializeModuleClass(Boo.Lang.Compiler.Ast.ClassDefinition moduleClass)
		{
			_moduleClassNamespace = (INamespace) _provider.EntityFor(moduleClass);
			_moduleClass = moduleClass;
		}
		
		public bool ResolveMember(ICollection<IEntity> targetList, string name, EntityType flags)
		{
			if (ResolveModuleMember(targetList, name, flags))
				return true;
			return _moduleClassNamespace.Resolve(targetList, name, flags);
		}
		
		public INamespace ParentNamespace
		{
			get
			{
				return _provider.EntityFor((CompileUnit)_module.ParentNode).RootNamespace.ParentNamespace;
			}
		}

		public IEnumerable<Import> Imports
		{
			get { return _module.Imports; }
		}

		public bool Resolve(ICollection<IEntity> resultingSet, string name, EntityType typesToConsider)
		{
			if (ResolveMember(resultingSet, name, typesToConsider))
				return true;

			bool found = false;
			foreach (INamespace importedNamespace in ImportedNamespaces())
				if (importedNamespace.Resolve(resultingSet, name, typesToConsider))
					found = true;
			return found;
		}

		private IEnumerable<INamespace> ImportedNamespaces()
		{
			foreach (Import import in _module.Imports)
			{
				IEntity entity = import.Entity;
				if (entity != null)
					yield return (INamespace) entity;
			}
		}

		bool ResolveModuleMember(ICollection<IEntity> targetList, string name, EntityType flags)
		{
			bool found = false;
			foreach (TypeMember member in _module.Members)
			{
				if (name != member.Name) continue;

				IEntity entity = _provider.EntityFor(member);
				if (Entities.IsFlagSet(flags, entity.EntityType))
				{
					targetList.Add(entity);
					found = true;
				}
			}
			return found;
		}
		
		public IEnumerable<IEntity> GetMembers()
		{
			yield break;
		}

		public static INamespace ScopeFor(Module module)
		{
			return ((InternalModule) TypeSystemServices.GetEntity(module));
		}
	}

	internal class ModuleMembersNamespace : AbstractNamespace
	{
		private InternalModule _module;

		public ModuleMembersNamespace(InternalModule module)
		{
			_module = module;
		}

		#region Overrides of AbstractNamespace

		public override INamespace ParentNamespace
		{
			get
			{
				return _module.ParentNamespace;
			}
		}

		public override bool Resolve(ICollection<IEntity> resultingSet, string name, EntityType typesToConsider)
		{
			return _module.ResolveMember(resultingSet, name, typesToConsider);
		}

		public override IEnumerable<IEntity> GetMembers()
		{
			return _module.GetMembers();
		}

		#endregion
	}
}