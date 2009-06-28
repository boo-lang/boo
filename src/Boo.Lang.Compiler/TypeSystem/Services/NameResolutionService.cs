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
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Steps;
using Boo.Lang.Compiler.TypeSystem.Core;
using Boo.Lang.Compiler.TypeSystem.Generics;
using Boo.Lang.Compiler.Util;

namespace Boo.Lang.Compiler.TypeSystem.Services
{
	public delegate bool EntityNameMatcher(IEntity candidate, string name);

	public class NameResolutionService
	{
		public static readonly char[] DotArray = new char[] { '.' };
		
		protected INamespace _current;
		
		protected INamespace _global = NullNamespace.Default;

		private EntityNameMatcher _entityNameMatcher = Matches;

		public EntityNameMatcher EntityNameMatcher
		{
			get { return _entityNameMatcher; }
			set
			{
				if (null == value) throw new ArgumentNullException();
				_entityNameMatcher = value;
			}
		}
		
		public INamespace GlobalNamespace
		{
			get { return _global; }
			
			set
			{
				if (null == value)
				{
					throw new ArgumentNullException("GlobalNamespace");
				}
				_global = value;
			}
		}		
		
		public void EnterNamespace(INamespace ns)
		{
			if (null == ns) throw new ArgumentNullException("ns");
			_current = ns;
		}
		
		public INamespace CurrentNamespace
		{
			get { return _current; }
		}
		
		public void Reset()
		{
			EnterNamespace(_global);
		}
		
		public void Restore(INamespace saved)
		{
			if (null == saved) throw new ArgumentNullException("saved");
			_current = saved;
		}
		
		public void LeaveNamespace()
		{
			_current = _current.ParentNamespace;
		}
		
		public IEntity Resolve(string name)
		{
			return Resolve(name, EntityType.Any);
		}
		
		public IEntity Resolve(string name, EntityType flags)
		{
			Set<IEntity> resultingSet = new Set<IEntity>();
			Resolve(resultingSet, name, flags);
			return Entities.EntityFromList(resultingSet);
		}
		
		public bool Resolve(ICollection<IEntity> targetList, string name)
		{
			return Resolve(targetList, name, EntityType.Any);
		}

		public IEnumerable<EntityOut> Select<EntityOut>(IEnumerable<IEntity> candidates, string name, EntityType typesToConsider)
		{
			foreach (IEntity entity in candidates)
				if (Matches(entity, name, typesToConsider))
					yield return (EntityOut) entity;
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
			return _entityNameMatcher(entity, name) && Entities.IsFlagSet(typesToConsider, entity.EntityType);
		}

		private static bool Matches(IEntity entity, string name)
		{
			return entity.Name == name;
		}

		public bool Resolve(ICollection<IEntity> targetList, string name, EntityType flags)
		{
			IEntity entity = My<TypeSystemServices>.Instance.ResolvePrimitive(name);
			if (null != entity)
			{
				targetList.Add(entity);
				return true;
			}

			AssertInNamespace();
			INamespace current = _current;
			do
			{
				if (Namespaces.ResolveCoalescingNamespaces(current.ParentNamespace, current, name, flags, targetList))
					return true;
				current = current.ParentNamespace;
			} while (current != null);
			return false;
		}

		public IEntity ResolveExtension(INamespace ns, string name)
		{
			IType type = ns as IType;
			if (null == type) return null;
			
			INamespace current = _current;
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
			Set<IEntity> extensions = new Set<IEntity>();
			if (!ns.Resolve(extensions, name, EntityType.Method | EntityType.Property))
				return null;

			Predicate<IEntity> notExtensionPredicate = delegate(IEntity item)
			{
				return !IsExtensionOf(type, item as IExtensionEnabled);
			};

			extensions.RemoveAll(notExtensionPredicate);
			return Entities.EntityFromList(extensions);
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
			if (extensionType.IsAssignableFrom(actualType)) return true;

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
		
		public bool ResolveQualifiedName(ICollection<IEntity> targetList, string name)
		{
			return ResolveQualifiedName(targetList, name, EntityType.Any);
		}

		public IEntity ResolveQualifiedName(string name, EntityType flags)
		{
			Set<IEntity> resultingSet = new Set<IEntity>();
			ResolveQualifiedName(resultingSet, name, flags);
			return Entities.EntityFromList(resultingSet);
		}

		public bool ResolveQualifiedName(ICollection<IEntity> targetList, string name, EntityType flags)
		{
			if (!IsQualifiedName(name))
				return Resolve(targetList, name, flags);

			AssertInNamespace();
			INamespace current = _current;
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
			
			IEntity entity = ResolveTypeName(node);
			if (null == entity)
			{
				node.Entity = NameNotType(node, "not found");
				return;
			}
			GenericTypeReference gtr = node as GenericTypeReference;
			if (null != gtr)
			{
				entity = ResolveGenericTypeReference(gtr, entity);
			}

			GenericTypeDefinitionReference gtdr = node as GenericTypeDefinitionReference;
			if (null != gtdr)
			{
				IType type = (IType)entity;
				if (gtdr.GenericPlaceholders != type.GenericInfo.GenericParameters.Length)
				{
					GenericArgumentsCountMismatch(gtdr, type);
					return;
				}
			}

			entity = Entities.PreferInternalEntitiesOverExternalOnes(entity);

			if (EntityType.Type != entity.EntityType)
			{
				if (EntityType.Ambiguous == entity.EntityType)
				{
					entity = AmbiguousReference(node, (Ambiguous)entity);
				}
				else if (EntityType.Error != entity.EntityType)
				{
					entity = NameNotType(node, entity.ToString());
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
			Set<IEntity> resultingSet = new Set<IEntity>();
			if (IsQualifiedName(node.Name))
			{
				ResolveQualifiedName(resultingSet, node.Name);
			}
			else
			{
				Resolve(resultingSet, node.Name, EntityType.Type);
			}


			// Remove from the buffer types that do not match requested generity
			FilterGenericTypes(resultingSet, node);
			return Entities.EntityFromList(resultingSet);
		}

		public IType ResolveGenericTypeReference(GenericTypeReference gtr, IEntity definition)
		{
			ResolveTypeReferenceCollection(gtr.GenericArguments);
			IType[] typeArguments = GetTypes(gtr.GenericArguments);
			
			return (IType)My<GenericsServices>.Instance.ConstructEntity(
			              	gtr, definition, typeArguments);
		}

		public IEntity ResolveGenericReferenceExpression(GenericReferenceExpression gre, IEntity definition)
		{
			ResolveTypeReferenceCollection(gre.GenericArguments);
			IType[] typeArguments = GetTypes(gre.GenericArguments);
			
			return My<GenericsServices>.Instance.ConstructEntity(
				gre, definition, typeArguments);
		}

		private IType[] GetTypes(TypeReferenceCollection typeReferences)
		{
			return Array.ConvertAll<TypeReference, IType>(
				typeReferences.ToArray(),
				TypeSystemServices.GetType);
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

		private bool IsNotGenericType(IEntity entity)
		{
			IType type = entity as IType;
			return type != null && type.GenericInfo == null;
		}

		private IEntity NameNotType(SimpleTypeReference node, string whatItIs)
		{
			string suggestion = GetMostSimilarTypeName(node.Name);
			CompilerErrors().Add(CompilerErrorFactory.NameNotType(node, node.ToCodeString(), whatItIs, suggestion));
			return TypeSystemServices.ErrorEntity;
		}

		private CompilerErrorCollection CompilerErrors()
		{
			return CompilerContext.Current.Errors;
		}

		private IEntity AmbiguousReference(SimpleTypeReference node, Ambiguous entity)
		{
			CompilerErrors().Add(CompilerErrorFactory.AmbiguousReference(node, node.Name, entity.Entities));
			return TypeSystemServices.ErrorEntity;
		}
		
		private void GenericArgumentsCountMismatch(TypeReference node, IType type)
		{
			CompilerErrors().Add(CompilerErrorFactory.GenericDefinitionArgumentCount(node, type.FullName, type.GenericInfo.GenericParameters.Length));
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
			Set<IEntity> resultingSet = new Set<IEntity>();
			ResolveCoalescingNamespaces(@namespace, name, elementType, resultingSet);
			return Entities.EntityFromList(resultingSet);
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
			Set<IEntity> resultingSet = new Set<IEntity>();
			ResolveQualifiedNameAgainst(namespaceToResolveAgainst, name, EntityType.Any, resultingSet);
			return Entities.EntityFromList(resultingSet);
		}
	}
}
