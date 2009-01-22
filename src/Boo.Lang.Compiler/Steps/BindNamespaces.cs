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
using System.Reflection;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang;

namespace Boo.Lang.Compiler.Steps
{
	public class BindNamespaces : AbstractTransformerCompilerStep
	{
		private Hash nameSpaces = new Hash();
		
		override public void Run()
		{
			NameResolutionService.Reset();
			
			Visit(CompileUnit.Modules);
		}
		
		override public void OnModule(Boo.Lang.Compiler.Ast.Module module)
		{
			Visit(module.Imports);
			nameSpaces.Clear();
		}
		
		public override void OnImport(Boo.Lang.Compiler.Ast.Import import)
		{
			if (IsAlreadyBound(import))
				return;

			IEntity entity = ResolveImport(import);

			//if 'import X', try 'import X from X'
			//comment out next if block if this is not wanted
			if (null == entity && null == import.AssemblyReference)
			{
				if (TryAutoAddAssemblyReference(import))
				{
					entity = NameResolutionService.ResolveQualifiedName(import.Namespace);
				}
			}
			
			if (null == entity)
			{
				Errors.Add(CompilerErrorFactory.InvalidNamespace(import));
				entity = TypeSystemServices.ErrorEntity;
			}
			else
			{
				if (!IsValidNamespace(entity))
				{
					Errors.Add(CompilerErrorFactory.NotANamespace(import, entity.FullName));
					entity = TypeSystemServices.ErrorEntity;
				}
				else
				{
					string name = entity.FullName;
					if (null != import.AssemblyReference)
					{
						NamespaceEntity nsInfo = entity as NamespaceEntity;
						if (null != nsInfo)
						{
							entity = new AssemblyQualifiedNamespaceEntity(GetBoundAssembly(import.AssemblyReference), nsInfo);
						}
					}
					
					if (null != import.Alias)
					{
						entity = new AliasedNamespace(import.Alias.Name, entity);
						import.Alias.Entity = entity;
						name = entity.Name; //use alias name instead of namespace name
					}
					
					//only add unique namespaces
					Import cachedImport = nameSpaces[name] as Import;
					if (cachedImport == null)
					{
						nameSpaces[name] = import;
					}
					else
					{
						//ignore for partial classes in separate files
						if (cachedImport.LexicalInfo.FileName == import.LexicalInfo.FileName)
						{
							Warnings.Add(
								CompilerWarningFactory.DuplicateNamespace(import, import.Namespace));
						}
						RemoveCurrentNode();
						return;
					}
				}
			}
			
			_context.TraceInfo("{1}: import reference '{0}' bound to {2}.", import, import.LexicalInfo, entity.FullName);
			import.Entity = entity;
		}

		private bool IsAlreadyBound(Import import)
		{
			return TypeSystemServices.GetOptionalEntity(import) != null;
		}

		private IEntity ResolveImport(Import import)
		{
			return ResolveImportOnParentNamespace(import)
				?? NameResolutionService.ResolveQualifiedName(import.Namespace);
		}

		private IEntity ResolveImportOnParentNamespace(Import import)
		{	
			INamespace current = NameResolutionService.CurrentNamespace;
			try
			{
				INamespace parentNamespace = NameResolutionService.CurrentNamespace.ParentNamespace;
				if (parentNamespace != null)
				{
					NameResolutionService.EnterNamespace(parentNamespace);
					return NameResolutionService.ResolveQualifiedName(import.Namespace);
				}
			}
			finally
			{
				NameResolutionService.EnterNamespace(current);
			}
			return null;
		}

		private bool TryAutoAddAssemblyReference(Boo.Lang.Compiler.Ast.Import import)
		{
			Assembly asm = Parameters.FindAssembly(import.Namespace);
			if (asm != null) return false; //name resolution already failed earlier, don't try twice
			
			asm = Parameters.LoadAssembly(import.Namespace, false);
			if (asm == null) {
				//try generalized namespaces
				string[] namespaces = import.Namespace.Split(new char[]{'.',});
				if (namespaces.Length == 1) {
					return false;
				} else {
					string ns;
					int level = namespaces.Length - 1;
					while (level > 0) {
						ns = string.Join(".", namespaces, 0, level);
						asm = Parameters.FindAssembly(ns);
						if (asm != null) return false; //name resolution already failed earlier, don't try twice
						asm = Parameters.LoadAssembly(ns, false);
						if (asm != null) break;
						level--;
					}
				}
			}
			
			if (asm != null)
			{
				try
				{
					NameResolutionService.OrganizeAssemblyTypes(asm);
				}
				catch (Exception /*ignored*/)
				{
					return false;
				}
				Parameters.AddAssembly(asm);
				import.AssemblyReference = new ReferenceExpression(import.LexicalInfo, asm.FullName);
				import.AssemblyReference.Entity = new AssemblyReference(asm);
				return true;
			}
			return false;
		}
		
		private bool IsValidNamespace(IEntity entity)
		{
			EntityType type = entity.EntityType;
			return type == EntityType.Namespace || type == EntityType.Type;
		}
		
		Assembly GetBoundAssembly(ReferenceExpression reference)
		{
			return ((AssemblyReference)TypeSystemServices.GetEntity(reference)).Assembly;
		}
	}
}
