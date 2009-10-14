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

using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Internal;

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

			if (null != import.AssemblyReference)
			{
				ImportFromAssemblyReference(import);
				return;
			}

			IEntity entity = ResolveImport(import);
			if (HandledAsImportError(import, entity))
				return;

			if (HandledAsDuplicatedNamespace(import, entity))
				return;

			_context.TraceInfo("{1}: import reference '{0}' bound to {2}.", import, import.LexicalInfo, entity.FullName);
			import.Entity = ImportedNamespaceFor(import, entity);
		}

		private bool HandledAsImportError(Import import, IEntity entity)
		{
			if (null == entity)
			{
				Errors.Add(CompilerErrorFactory.InvalidNamespace(import));
				RemoveCurrentNode();
				return true;
			}

			if (!IsValidNamespace(entity))
			{
				Errors.Add(CompilerErrorFactory.NotANamespace(import, entity.FullName));
				RemoveCurrentNode();
				return true;
			}
			return false;
		}

		private string EffectiveNameForImportedNamespace(Import import)
		{
			return null != import.Alias
			       	? import.Alias.Name
			       	: import.Namespace;
		}

		private bool HandledAsDuplicatedNamespace(Import import, IEntity resolvedEntity)
		{
			string actualName = EffectiveNameForImportedNamespace(import);
			//only add unique namespaces
			Import cachedImport = nameSpaces[actualName] as Import;
			if (cachedImport == null)
			{
				nameSpaces[actualName] = import;
				return false;
			}

			//ignore for partial classes in separate files
			if (cachedImport.LexicalInfo.FileName == import.LexicalInfo.FileName)
			{
				Warnings.Add(
					CompilerWarningFactory.DuplicateNamespace(import, import.Namespace));
			}
			RemoveCurrentNode();
			return true;
		}

		private IEntity ImportedNamespaceFor(Import import, IEntity entity)
		{
			INamespace ns = entity as INamespace;
			if (null == ns)
				return entity;

			INamespace actualNamespace = null != import.Alias
			                        ? AliasedNamespaceFor(entity, import)
			                        : ns;
			return new ImportedNamespace(import, actualNamespace);
		}

		private INamespace AliasedNamespaceFor(IEntity entity, Import import)
		{
			INamespace aliasedNamespace = new AliasedNamespace(import.Alias.Name, entity);
			import.Alias.Entity = aliasedNamespace;
			return aliasedNamespace;
		}

		private void ImportFromAssemblyReference(Import import)
		{
			IEntity resolvedNamespace = ResolveImportAgainstReferencedAssembly(import);
			if (HandledAsImportError(import, resolvedNamespace))
				return;
			if (HandledAsDuplicatedNamespace(import, resolvedNamespace))
				return;
			import.Entity = ImportedNamespaceFor(import, resolvedNamespace);
		}

		private IEntity ResolveImportAgainstReferencedAssembly(Import import)
		{
			return NameResolutionService.ResolveQualifiedName(GetBoundReference(import.AssemblyReference).RootNamespace, import.Namespace);
		}

		private bool IsAlreadyBound(Import import)
		{
			return TypeSystemServices.GetOptionalEntity(import) != null;
		}

		private IEntity ResolveImport(Import import)
		{
			IEntity entity = ResolveImportOnParentNamespace(import)
			                  ?? NameResolutionService.ResolveQualifiedName(import.Namespace);
			if (null != entity)
				return entity;

			//if 'import X', try 'import X from X'
			if (!TryAutoAddAssemblyReference(import))
				return null;

			return NameResolutionService.ResolveQualifiedName(import.Namespace);
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
			ICompileUnit asm = Parameters.FindAssembly(import.Namespace);
			if (asm != null)
				return false; //name resolution already failed earlier, don't try twice
			
			asm = Parameters.LoadAssembly(import.Namespace, false);
			if (asm == null) {
				//try generalized namespaces
				string[] namespaces = import.Namespace.Split(new char[]{'.',});
				if (namespaces.Length == 1) {
					return false;
				}
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
			
			if (asm != null)
			{
				Parameters.References.Add(asm);
				import.AssemblyReference = new ReferenceExpression(import.LexicalInfo, asm.FullName);
				import.AssemblyReference.Entity = asm;
				return true;
			}
			return false;
		}
		
		private bool IsValidNamespace(IEntity entity)
		{
			EntityType type = entity.EntityType;
			return type == EntityType.Namespace || type == EntityType.Type;
		}

		static ICompileUnit GetBoundReference(ReferenceExpression reference)
		{
			return ((ICompileUnit)TypeSystemServices.GetEntity(reference));
		}
	}
}
