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
using System.Collections.Generic;
using System.Linq;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem.Core;
using Boo.Lang.Compiler.TypeSystem.Generics;
using Boo.Lang.Compiler.Util;
using Boo.Lang.Environments;

namespace Boo.Lang.Compiler.TypeSystem.Services
{
	public delegate bool EntityNameMatcher(IEntity candidate, string name);

	/// <summary>
	/// Means to find the entity of a type reference or to set
	/// the entity of type references (resolve the reference).
	/// </summary>
	public class NameResolutionService
	{
		public static readonly char[] DotArray = new char[] { '.' };
		
		protected INamespace _global;

		private EntityNameMatcher _entityNameMatcher = Matches;

		private readonly CurrentScope _current = My<CurrentScope>.Instance;

		private MemoizedFunction<string, IType, IEntity> _resolveExtensionFor;
		private MemoizedFunction<string, EntityType, IEntity> _resolveName;

		public NameResolutionService()
		{
			_resolveExtensionFor = new MemoizedFunction<string, IType, IEntity>(StringComparer.Ordinal, ResolveExtensionFor);
			_resolveName = new MemoizedFunction<string, EntityType, IEntity>(StringComparer.Ordinal, ResolveImpl);
			_current.Changed += (sender, args) => ClearResolutionCache();

			_global = My<GlobalNamespace>.Instance;
			Reset();
		}

		public EntityNameMatcher EntityNameMatcher
		{
			get { return _entityNameMatcher; }
			set
			{
				if (null == value)
					throw new ArgumentNullException();
				_entityNameMatcher = value;
			}
		}
		
		public INamespace GlobalNamespace
		{
			get { return _global; }
			
			set
			{
				if (null == value)
					throw new ArgumentNullException("GlobalNamespace");
				_global = value;
			}
		}		
		
		private Stack<MemoizedFunction<string, EntityType, IEntity>> _stackNameCaches = new Stack<MemoizedFunction<string, EntityType, IEntity>>();
		private Stack<MemoizedFunction<string, IType, IEntity>> _stackExtensionCaches = new Stack<MemoizedFunction<string, IType, IEntity>>();
		public void EnterNamespace(INamespace ns)
		{
			if (null == ns)
                throw new ArgumentNullException("ns");
			_stackNameCaches.Push(_resolveName);
			_stackExtensionCaches.Push(_resolveExtensionFor);
			_resolveName = _resolveName.Clone();
			_resolveExtensionFor = _resolveExtensionFor.Clone();
			CurrentNamespace = ns;
		}
		
		public INamespace CurrentNamespace
		{
			get { return _current.Value; }
			private set { _current.Value = value; }
		}
		
		public void Reset()
		{
			_stackNameCaches.Clear();
			_stackExtensionCaches.Clear();
			EnterNamespace(_global);
		}
		
		public void LeaveNamespace()
		{
			CurrentNamespace = CurrentNamespace.ParentNamespace;
			_resolveName = _stackNameCaches.Pop();
			_resolveExtensionFor = _stackExtensionCaches.Pop();
		}
		
		public IEntity Resolve(string name)
		{
			return Resolve(name, EntityType.Any);
		}

		public IEntity Resolve(string name, EntityType flags)
		{
			return _resolveName.Invoke(name, flags);
		}

		private IEntity ResolveImpl(string name, EntityType flags)
		{
			var resultingSet = Namespaces.AcquireSet();
			try {
			Resolve(resultingSet, name, flags);
			return Entities.EntityFromList(resultingSet);
			}
			finally {
				Namespaces.ReleaseSet(resultingSet);
			}
		}

		public void ClearResolutionCacheFor(string name)
		{
			_resolveName.Clear(name);
		}

		void ClearResolutionCache()
		{
			_resolveName.Clear();
			_resolveExtensionFor.Clear();
		}
				
		public IEnumerable<TEntityOut> Select<TEntityOut>(IEnumerable<IEntity> candidates, string name, EntityType typesToConsider)
		{
			var result = new List<TEntityOut>();
			foreach (var candidate in candidates)
				if (Matches(candidate, name, typesToConsider))
					result.Add((TEntityOut) candidate);
			return result;
		}

	    public bool Resolve(string name, IEnumerable<IEntity> candidates, EntityType typesToConsider, ICollection<IEntity> resolvedSet)
		{
			bool found = false;
			foreach (IEntity entity in Select<IEntity>(candidates, name, typesToConsider))
			{
				resolvedSet.Add(entity);
				found = true;
			}
			return found;
		}

		private bool Matches(IEntity entity, string name, EntityType typesToConsider)
		{
			return Entities.IsFlagSet(typesToConsider, entity.EntityType) && _entityNameMatcher(entity, name);
		}

		private static bool Matches(IEntity entity, string name)
		{
			return entity.Name == name;
		}

		private void Resolve(ICollection<IEntity> targetList, string name, EntityType flags)
		{
			var entity = My<TypeSystemServices>.Instance.ResolvePrimitive(name);
			if (entity != null)
			{
				targetList.Add(entity);
				return;
			}

			AssertInNamespace();
			var current = CurrentNamespace;
			do
			{
				if (Namespaces.ResolveCoalescingNamespaces(current.ParentNamespace, current, name, flags, targetList))
					return;
				current = current.ParentNamespace;
			}
			while (current != null);
		}

		public IEntity ResolveExtension(INamespace ns, string name)
		{
			var type = ns as IType;
			if (null == type) return null;
			return _resolveExtensionFor.Invoke(name, type);
		}

		private IEntity ResolveExtensionFor(string name, IType type)
		{
			INamespace current = CurrentNamespace;
			while (null != current)
			{
				IEntity found = ResolveExtensionForType(current, type, name);
				if (null != found) return found;
				current = current.ParentNamespace;
			}
			return null;
		}

		private IEntity ResolveExtensionForType(INamespace ns, IType type, string name)
		{
			var extensions = Namespaces.AcquireSet();
			try {
			if (!ns.Resolve(extensions, name, EntityType.Method | EntityType.Property))
				return null;

			Predicate<IEntity> notExtensionPredicate = item => !IsExtensionOf(type, item as IExtensionEnabled);
			extensions.RemoveAll(notExtensionPredicate);
			return Entities.EntityFromList(extensions);
			}
			finally {
				Namespaces.ReleaseSet(extensions);
			}
		}

		private bool IsExtensionOf(IType type, IExtensionEnabled entity)
		{
			if (entity == null || !entity.IsExtension) return false;

			IParameter[] parameters = entity.GetParameters();
			if (parameters.Length == 0) return false;

			IType extensionType = parameters[0].Type;
			return IsValidExtensionType(type, extensionType, entity);
		}

		private bool IsValidExtensionType(IType actualType, IType extensionType, IExtensionEnabled extension)
		{
			if (TypeCompatibilityRules.IsAssignableFrom(extensionType, actualType)) return true;

			// Check for a valid generic extension
			IMethod method = extension as IMethod;
			if (method == null || method.GenericInfo == null) return false;

			System.Collections.Generic.List<IGenericParameter> genericParameters = new System.Collections.Generic.List<IGenericParameter>(GenericsServices.FindGenericParameters(extensionType));
			if (genericParameters.Count == 0) return false;

			TypeInferrer inferrer = new TypeInferrer(genericParameters);
			inferrer.Infer(extensionType, actualType);
			return inferrer.FinalizeInference();
		}

		public IEntity ResolveQualifiedName(string name)
		{	
			return ResolveQualifiedName(name, EntityType.Any);
		}

		private IEntity ResolveQualifiedName(string name, EntityType flags)
		{	
			if (!IsQualifiedName(name))
				return Resolve(name, flags);

			var resultingSet = Namespaces.AcquireSet();
			try {
			ResolveQualifiedName(resultingSet, name, flags);
			return Entities.EntityFromList(resultingSet);
			}
			finally {
				Namespaces.ReleaseSet(resultingSet);
			}
		}

		private bool ResolveQualifiedName(ICollection<IEntity> targetList, string name, EntityType flags)
		{
			AssertInNamespace();
			INamespace current = CurrentNamespace;
			do
			{
				if (ResolveQualifiedNameAgainst(current, name, flags, targetList))
					return true;
				current = current.ParentNamespace;
			} while (current != null);

			return false;
		}

		private bool ResolveQualifiedNameAgainst(INamespace current, string name, EntityType flags, ICollection<IEntity> resultingSet)
		{
			string[] parts = name.Split(DotArray);
			for (int i=0; i<parts.Length - 1; ++i)
			{
				current = Resolve(current, parts[i], EntityType.Namespace | EntityType.Type) as INamespace;
				if (current == null)
					return false;
			}
			return ResolveCoalescingNamespaces(current, parts[parts.Length-1], flags, resultingSet);
		}

		private void AssertInNamespace()
		{
			if (_current == null)
				throw new InvalidOperationException("No namespace.");
		}

		public void ResolveTypeReference(TypeReference node)
		{
			if (null != node.Entity)
				return;

			switch (node.NodeType)
			{
				case NodeType.ArrayTypeReference:
					ResolveArrayTypeReference((ArrayTypeReference) node);
					break;

				case NodeType.CallableTypeReference:
					//not needed? (late resolution)
					//ResolveCallableTypeReference((CallableTypeReference) node);
					break;

				default:
					ResolveSimpleTypeReference((SimpleTypeReference) node);
					break;
			}
		}

		public void ResolveArrayTypeReference(ArrayTypeReference node)
		{
			if (null != node.Entity) return;

			ResolveTypeReference(node.ElementType);
			
			IType elementType = TypeSystemServices.GetType(node.ElementType);
			if (TypeSystemServices.IsError(elementType))
			{
				node.Entity = TypeSystemServices.ErrorEntity;
			}
			else
			{
				int rank = null == node.Rank ? 1 : (int)node.Rank.Value;
				node.Entity = elementType.MakeArrayType(rank);
			}
		}

		private void ResolveTypeReferenceCollection(TypeReferenceCollection collection)
		{
			foreach (TypeReference tr in collection)
			{
				ResolveTypeReference(tr);
			}
		}
		
		public void ResolveSimpleTypeReference(SimpleTypeReference node)
		{
			if (null != node.Entity) return;
			
			var entity = ResolveQualifiedName(node.Name, EntityType.Type);
			
			if (entity == null)
			{
				node.Entity = NameNotType(node, null);
				return;
			}
			
			IEntity firstCandidate = null;
			if (entity.IsAmbiguous())
			{
				// Remove from the buffer types that do not match requested generity
				var resultingSet = new Set<IEntity>(((Ambiguous)entity).Entities);
				firstCandidate = resultingSet.First();
				FilterGenericTypes(resultingSet, node);
				entity = Entities.EntityFromList(resultingSet);
			}		

			if (NodeType.SimpleTypeReference == node.NodeType)
			{
				if (IsGenericType(entity)) {
					firstCandidate = entity;
					entity = null;
				}
				
				if (entity == null)
				{
					//Generic parameters are missing because there is no candidates after filtering out generic types
					GenericArgumentsCountMismatch(node, firstCandidate as IType);
					node.Entity = TypeSystemServices.ErrorEntity;
					return;
				}
			}
			
			if (NodeType.GenericTypeReference == node.NodeType)
			{				
				var gtr = node as GenericTypeReference;
				entity = ResolveGenericTypeReference(gtr, entity);
			}

			
			if (NodeType.GenericTypeDefinitionReference == node.NodeType)
			{
				var gtdr = node as GenericTypeDefinitionReference;
				IType type = (IType)entity;
				if (gtdr.GenericPlaceholders != type.GenericInfo.GenericParameters.Length)
				{
					GenericArgumentsCountMismatch(gtdr, type);
					node.Entity = TypeSystemServices.ErrorEntity;
					return;
				}
			}

			entity = Entities.PreferInternalEntitiesOverExternalOnes(entity);

			if (EntityType.Type != entity.EntityType)
			{
				if (entity.IsAmbiguous())
				{
					entity = AmbiguousReference(node, (Ambiguous)entity);
				}
				else if (EntityType.Error != entity.EntityType)
				{
					entity = NameNotType(node, entity);
				}
			}
			else
			{
				node.Name = entity.FullName;
			}

			if (node.IsPointer && EntityType.Type == entity.EntityType)
				entity = ((IType) entity).MakePointerType();

			node.Entity = entity;
		}

		internal IEntity ResolveTypeName(SimpleTypeReference node)
		{
			var resolved = ResolveQualifiedName(node.Name, EntityType.Type);
			if (resolved == null)
				return null;
			if (resolved.IsAmbiguous())
			{
				// Remove from the buffer types that do not match requested generity
				var resultingSet = new Set<IEntity>(((Ambiguous)resolved).Entities);
				FilterGenericTypes(resultingSet, node);
				return Entities.EntityFromList(resultingSet);
			}
			return resolved;
		}

		public IEntity ResolveGenericTypeReference(GenericTypeReference gtr, IEntity definition)
		{
			ResolveTypeReferenceCollection(gtr.GenericArguments);
			IType[] typeArguments = gtr.GenericArguments.ToArray(t => TypeSystemServices.GetType(t));
			
			return My<GenericsServices>.Instance.ConstructEntity(gtr, definition, typeArguments);
		}

		public IEntity ResolveGenericReferenceExpression(GenericReferenceExpression gre, IEntity definition)
		{
			ResolveTypeReferenceCollection(gre.GenericArguments);
			IType[] typeArguments = gre.GenericArguments.ToArray(t => TypeSystemServices.GetType(t));
			
			return My<GenericsServices>.Instance.ConstructEntity(
				gre, definition, typeArguments);
		}

		private void FilterGenericTypes(Set<IEntity> types, SimpleTypeReference node)		
		{			
			bool genericRequested = (node is GenericTypeReference || node is GenericTypeDefinitionReference);
			if (genericRequested)
				types.RemoveAll(IsNotGenericType);
			else
				types.RemoveAll(IsGenericType);
		}

		private bool IsGenericType(IEntity entity)
		{
			IType type = entity as IType;
			return type != null && type.GenericInfo != null;
		}

		private static bool IsNotGenericType(IEntity entity)
		{
			IType type = entity as IType;
			return type != null && type.GenericInfo == null;
		}

		private IEntity NameNotType(SimpleTypeReference node, IEntity actual)
		{
			string suggestion = GetMostSimilarTypeName(node.Name);
			CompilerErrors().Add(CompilerErrorFactory.NameNotType(node, node.Name, actual, suggestion));
			return TypeSystemServices.ErrorEntity;
		}

		private CompilerErrorCollection CompilerErrors()
		{
            return My<CompilerErrorCollection>.Instance;
		}

		private IEntity AmbiguousReference(SimpleTypeReference node, Ambiguous entity)
		{
			CompilerErrors().Add(CompilerErrorFactory.AmbiguousReference(node, node.Name, entity.Entities));
			return TypeSystemServices.ErrorEntity;
		}
		
		private void GenericArgumentsCountMismatch(TypeReference node, IType type)
		{
		    CompilerErrorEmitter().GenericArgumentsCountMismatch(node, type);
		}

		private CompilerErrorEmitter CompilerErrorEmitter()
		{
			return My<CompilerErrorEmitter>.Instance;
		}

		public IField ResolveField(IType type, string name)
		{
			return (IField)ResolveMember(type, name, EntityType.Field);
		}
		
		public IMethod ResolveMethod(IType type, string name)
		{
			return (IMethod)ResolveMember(type, name, EntityType.Method);
		}
		
		public IProperty ResolveProperty(IType type, string name)
		{
			return (IProperty)ResolveMember(type, name, EntityType.Property);
		}
		
		public IEntity ResolveMember(IType type, string name, EntityType elementType)
		{
			foreach (IEntity member in type.GetMembers())
			{				
				if (elementType == member.EntityType && _entityNameMatcher(member, name))
				{
					return member;
				}
			}
			return null;
		}
		
		public IEntity Resolve(INamespace @namespace, string name, EntityType elementType)
		{
			if (@namespace == null)
				throw new ArgumentNullException("namespace");
			Set<IEntity> resultingSet = Namespaces.AcquireSet();
			try {
			ResolveCoalescingNamespaces(@namespace, name, elementType, resultingSet);
			return Entities.EntityFromList(resultingSet);
			}
			finally {
				Namespaces.ReleaseSet(resultingSet);
			}
		}

		private bool ResolveCoalescingNamespaces(INamespace ns, string name, EntityType elementType, ICollection<IEntity> resultingSet)
		{
			return Namespaces.ResolveCoalescingNamespaces(ns.ParentNamespace, ns, name, elementType, resultingSet);
		}

		public IEntity Resolve(INamespace ns, string name)
		{
			return Resolve(ns, name, EntityType.Any);
		}

		static bool IsQualifiedName(string name)
		{
			return name.IndexOf('.') > 0;
		}

		GlobalNamespace GetGlobalNamespace()
		{
			INamespace ns = _global;
			GlobalNamespace globals = ns as GlobalNamespace;
			while (globals == null && ns != null)
			{
				ns = ns.ParentNamespace;
				globals = ns as GlobalNamespace;
			}
			return globals;
		}

		private static void FlattenChildNamespaces(ICollection<INamespace> resultingList, INamespace ns)
		{
			foreach (IEntity ent in ns.GetMembers())
			{
				if (EntityType.Namespace != ent.EntityType)
					continue;
				INamespace nsEnt = (INamespace) ent;
				resultingList.Add(nsEnt);
				FlattenChildNamespaces(resultingList, nsEnt);
			}
		}

		public string GetMostSimilarTypeName(string name)
		{
			string[] namespaceHierarchy = name.Split('.');
			int nshLen = namespaceHierarchy.Length;
			string suggestion = null;

			if (nshLen > 1)
			{
				INamespace ns = null;
				INamespace prevNs = null;
				for (int i = 1; i < nshLen; i++)
				{
					string currentNsName = string.Join(".", namespaceHierarchy, 0, i);
					ns = ResolveQualifiedName(currentNsName) as INamespace;
					if (null == ns)
					{
						namespaceHierarchy[i-1] = GetMostSimilarMemberName(prevNs, namespaceHierarchy[i-1], EntityType.Namespace);
						if (null == namespaceHierarchy[i-1]) break;
						i--; continue; //reloop to resolve step
					}
					prevNs = ns;
				}
				suggestion = GetMostSimilarMemberName(ns, namespaceHierarchy[nshLen-1], EntityType.Type);
				if (null != suggestion)
				{
					namespaceHierarchy[nshLen-1] = suggestion;
					return string.Join(".", namespaceHierarchy);
				}
			}
		
			System.Collections.Generic.List<INamespace> nsList = new System.Collections.Generic.List<INamespace>();
			FlattenChildNamespaces(nsList, GetGlobalNamespace());
			nsList.Reverse();//most recently added namespaces first
			foreach (INamespace nse in nsList)
			{
				suggestion = GetMostSimilarMemberName(nse, namespaceHierarchy[nshLen-1], EntityType.Type);
				if (null != suggestion) return nse.ToString()+"."+suggestion;
			}
			return GetMostSimilarMemberName(GetGlobalNamespace(), namespaceHierarchy[nshLen-1], EntityType.Type);
		}

		public string GetMostSimilarMemberName(INamespace ns, string name, EntityType elementType)
		{
			if (null == ns) return null;

			string expectedSoundex = StringUtilities.GetSoundex(name);
			string lastMemberName = null;
			foreach (IEntity member in ns.GetMembers())
			{
				if (EntityType.Any != elementType && elementType != member.EntityType)
					continue;
				if (lastMemberName == member.Name)
					continue;//no need to check this name again
				//TODO: try Levenshtein distance or Metaphone instead of Soundex.
				if (expectedSoundex == StringUtilities.GetSoundex(member.Name))
				{
					//return properties without get_/set_ prefix
					IMethod method = member as IMethod;
					if (null != method && method.IsSpecialName)
						return member.Name.Substring(4);
					return member.Name;
				}
				lastMemberName = member.Name;
			}
			return null;
		}

		public IEntity ResolveQualifiedName(INamespace namespaceToResolveAgainst, string name)
		{
			Set<IEntity> resultingSet = Namespaces.AcquireSet();
			try {
			ResolveQualifiedNameAgainst(namespaceToResolveAgainst, name, EntityType.Any, resultingSet);
			return Entities.EntityFromList(resultingSet);
			}
			finally {
				Namespaces.ReleaseSet(resultingSet);
			}
		}
	}
}
