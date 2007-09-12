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

using System.Collections.Generic;
using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.TypeSystem
{
	public class ModuleEntity : INamespace, IEntity
	{
		private readonly NameResolutionService _nameResolutionService;
		
		private readonly TypeSystemServices _typeSystemServices;
		
		private readonly Module _module;
		
		private ClassDefinition _moduleClass;
		
		private INamespace _moduleClassNamespace = NullNamespace.Default;
		
		private string _namespace;
		
		public ModuleEntity(NameResolutionService nameResolutionService,
						TypeSystemServices typeSystemServices,
						Module module)
		{
			_nameResolutionService = nameResolutionService;
			_typeSystemServices = typeSystemServices;
			_module = module;			
			_namespace = SafeNamespace(module);
		}

		private static string SafeNamespace(Module module)
		{
			return null == module.Namespace ? string.Empty : module.Namespace.Name;
		}

		public EntityType EntityType
		{
			get { return EntityType.Module; }
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
		
		public void InitializeModuleClass(Boo.Lang.Compiler.Ast.ClassDefinition moduleClass)
		{
			_moduleClassNamespace = (INamespace) _typeSystemServices.GetMemberEntity(moduleClass);
			_moduleClass = moduleClass;
		}
		
		public bool ResolveMember(Boo.Lang.List targetList, string name, EntityType flags)
		{
			if (ResolveModuleMember(targetList, name, flags)) return true;
			return _moduleClassNamespace.Resolve(targetList, name, flags);
		}
		
		public INamespace ParentNamespace
		{
			get { return _nameResolutionService.GlobalNamespace; }
		}
		
		public bool Resolve(Boo.Lang.List targetList, string name, EntityType flags)
		{
			if (ResolveMember(targetList, name, flags)) return true;
				
			bool found = false;
			foreach (INamespace ns in ImportedNamespaces())
			{			
				found |= ns.Resolve(targetList, name, flags);
			}
			return found;
		}

		private IEnumerable<INamespace> ImportedNamespaces()
		{
			foreach (Import import in _module.Imports)
			{
				yield return (INamespace)TypeSystemServices.GetEntity(import);
			}
		}

		bool ResolveModuleMember(Boo.Lang.List targetList, string name, EntityType flags)
		{
			bool found = false;
			foreach (TypeMember member in _module.Members)
			{
				if (name != member.Name) continue;

				IEntity entity = _typeSystemServices.GetMemberEntity(member);
				if (NameResolutionService.IsFlagSet(flags, entity.EntityType))
				{
					targetList.Add(entity);
					found = true;
				}
			}
			return found;
		}
		
		public IEntity[] GetMembers()
		{
			return _moduleClassNamespace.GetMembers();
		}
	}
}
