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
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Core;
using Boo.Lang.Compiler.TypeSystem.Internal;

namespace Boo.Lang.Compiler.Steps
{
	public class ResolveImports : AbstractTransformerCompilerStep
	{
		private readonly Dictionary<string, Import> _namespaces = new Dictionary<string, Import>();
		
		private readonly Dictionary<INamespace, Dictionary<string, IEntity>> _cache = new Dictionary<INamespace, Dictionary<string, IEntity>>();
		
		override public void Run()
		{
			NameResolutionService.Reset();
			
			Visit(CompileUnit.Modules);
		}
		
		override public void OnModule(Module module)
		{
			Visit(module.Imports);
			_namespaces.Clear();
		}
		
		public override void OnImport(Import import)
		{
			if (IsAlreadyBound(import))
				return;

			if (import.AssemblyReference != null)
			{
				ImportFromAssemblyReference(import);
				return;
			}

			var entity = ResolveImport(import);
			if (HandledAsImportError(import, entity) || HandledAsDuplicatedNamespace(import))
				return;

			Context.TraceInfo("{1}: import reference '{0}' bound to {2}.", import, import.LexicalInfo, entity);
			import.Entity = ImportedNamespaceFor(import, entity);
		}

		private bool HandledAsImportError(Import import, IEntity entity)
		{
			if (entity == null)
			{
				ImportError(import, CompilerErrorFactory.InvalidNamespace(import));
				return true;
			}

			if (!IsValidImportTarget(entity))
			{
				ImportError(import, CompilerErrorFactory.NotANamespace(import, entity));
				return true;
			}

			return false;
		}

		private void ImportError(Import import, CompilerError error)
		{
			Errors.Add(error);
			BindError(import);
		}

		private void BindError(Import import)
		{
			Bind(import, Error.Default);
		}

		private static string EffectiveNameForImportedNamespace(Import import)
		{
			return null != import.Alias ? import.Alias.Name : import.Namespace;
		}

		private bool HandledAsDuplicatedNamespace(Import import)
		{
			var actualName = EffectiveNameForImportedNamespace(import);

			//only add unique namespaces
			Import cachedImport;
			if (!_namespaces.TryGetValue(actualName, out cachedImport))
			{
				_namespaces[actualName] = import;
				return false;
			}

			//ignore for partial classes in separate files
			if (cachedImport.LexicalInfo.FileName == import.LexicalInfo.FileName)
				Warnings.Add(CompilerWarningFactory.DuplicateNamespace(import, import.Namespace));

			BindError(import);
			return true;
		}

		private IEntity ImportedNamespaceFor(Import import, IEntity entity)
		{
			var ns = entity as INamespace;
			if (ns == null)
				return entity;

			var selectiveImportSpec = import.Expression as MethodInvocationExpression;
			var imported = selectiveImportSpec != null ? SelectiveImportFor(ns, selectiveImportSpec) : ns;
			var actualNamespace = null != import.Alias ? AliasedNamespaceFor(imported, import) : imported;
			return new ImportedNamespace(import, actualNamespace);
		}

		private INamespace SelectiveImportFor(INamespace ns, MethodInvocationExpression selectiveImportSpec)
		{
			var importedNames = selectiveImportSpec.Arguments;
			var entities = new List<IEntity>(importedNames.Count);
			var aliases = new Dictionary<string,string>(importedNames.Count);
			foreach (Expression nameExpression in importedNames)
			{
				string name;
				if (nameExpression is ReferenceExpression) {
					name = (nameExpression as ReferenceExpression).Name;
					aliases[name] = name;
				} else {
					var tce = nameExpression as TryCastExpression;
					var alias = (tce.Type as SimpleTypeReference).Name;
					name = (tce.Target as ReferenceExpression).Name;
					aliases[alias] = name;
					// Remove the trycast expression, otherwise it gets processed in later steps
					tce.Target.Annotate("alias", alias);
					importedNames.Replace(nameExpression, tce.Target);
				}

				if (!ns.Resolve(entities, name, EntityType.Any))
					Errors.Add(
						CompilerErrorFactory.MemberNotFound(
							nameExpression, name, ns, NameResolutionService.GetMostSimilarMemberName(ns, name, EntityType.Any)));
			}
			return new SimpleNamespace(null, entities, aliases);
		}

		private static INamespace AliasedNamespaceFor(IEntity entity, Import import)
		{
			var aliasedNamespace = new AliasedNamespace(import.Alias.Name, entity);
			import.Alias.Entity = aliasedNamespace;
			return aliasedNamespace;
		}

		private void ImportFromAssemblyReference(Import import)
		{
			var resolvedNamespace = ResolveImportAgainstReferencedAssembly(import);
			if (HandledAsImportError(import, resolvedNamespace))
				return;
			if (HandledAsDuplicatedNamespace(import))
				return;
			import.Entity = ImportedNamespaceFor(import, resolvedNamespace);
		}

		private IEntity ResolveImportAgainstReferencedAssembly(Import import)
		{
			return NameResolutionService.ResolveQualifiedName(GetBoundReference(import.AssemblyReference).RootNamespace, import.Namespace);
		}

		private static bool IsAlreadyBound(Import import)
		{
			return import.Entity != null;
		}

		private IEntity ResolveImport(Import import)
		{
			var entity = ResolveImportOnParentNamespace(import) ?? NameResolutionService.ResolveQualifiedName(import.Namespace);
			if (null != entity)
				return entity;

			//if 'import X', try 'import X from X'
			if (!TryAutoAddAssemblyReference(import))
				return null;

			return NameResolutionService.ResolveQualifiedName(import.Namespace);
		}

		private bool RetrieveCachedNamespace(INamespace parentNamespace, string name, out IEntity result)
		{
			 Dictionary<string, IEntity> subcache;
			 if (!_cache.TryGetValue(parentNamespace, out subcache))
			 {
			 	result = null;
			 	return false;
			 }
			 return subcache.TryGetValue(name, out result);
		}

		private void AddCachedNamespace(INamespace parentNamespace, string name, IEntity value)
		{
			 Dictionary<string, IEntity> subcache;
			 if (!_cache.TryGetValue(parentNamespace, out subcache))
			 {
			 	subcache = new Dictionary<string, IEntity>();
			 	_cache.Add(parentNamespace, subcache);
			 }
			 subcache.Add(name, value);
		}

		private IEntity ResolveImportOnParentNamespace(Import import)
		{	
			var current = NameResolutionService.CurrentNamespace;
			INamespace parentNamespace = NameResolutionService.CurrentNamespace.ParentNamespace;
			if (parentNamespace != null)
			{
				IEntity result;
				if (!RetrieveCachedNamespace(parentNamespace, import.Namespace, out result))
				{
					NameResolutionService.EnterNamespace(parentNamespace);
					try
					{
						result = NameResolutionService.ResolveQualifiedName(import.Namespace);
						AddCachedNamespace(parentNamespace, import.Namespace, result);
					}
					finally
					{
						NameResolutionService.EnterNamespace(current);
					}
				}
				return result;
			}
			return null;
		}

		private bool TryAutoAddAssemblyReference(Import import)
		{
			var existingReference = Parameters.FindAssembly(import.Namespace);
			if (existingReference != null)
				return false;

			var asm = TryToLoadAssemblyContainingNamespace(import.Namespace);
			if (asm == null)
				return false;

			Parameters.References.Add(asm);
			import.AssemblyReference = new ReferenceExpression(import.LexicalInfo, asm.FullName).WithEntity(asm);
			NameResolutionService.ClearResolutionCacheFor(asm.Name);
			return true;
		}

		private ICompileUnit TryToLoadAssemblyContainingNamespace(string @namespace)
		{
			ICompileUnit asm = Parameters.LoadAssembly(@namespace, false);
			if (asm != null)
				return asm;

			//try to load assemblies name after the parent namespaces
			var namespaces = @namespace.Split('.');
			if (namespaces.Length == 1)
				return null;

			for (var level = namespaces.Length - 1; level > 0; level--)
			{
				var parentNamespace = string.Join(".", namespaces, 0, level);
				var existingReference = Parameters.FindAssembly(parentNamespace);
				if (existingReference != null)
					return null;
				var parentNamespaceAssembly = Parameters.LoadAssembly(parentNamespace, false);
				if (parentNamespaceAssembly != null)
					return parentNamespaceAssembly;
			}
			return null;
		}

		private static bool IsValidImportTarget(IEntity entity)
		{
			var type = entity.EntityType;
			return type == EntityType.Namespace || type == EntityType.Type;
		}

		static ICompileUnit GetBoundReference(ReferenceExpression reference)
		{
			return ((ICompileUnit)TypeSystemServices.GetEntity(reference));
		}
	}
}
