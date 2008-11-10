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

namespace Boo.Lang.Compiler.TypeSystem
{
	using System;
	using System.Collections;
	using Boo.Lang.Compiler.Ast;
	using System.Reflection;
	using System.Collections.Generic;
	
	public class NameResolutionService
	{
		public static readonly char[] DotArray = new char[] { '.' };
		
		protected CompilerContext _context;
		
		protected INamespace _current;
		
		protected INamespace _global = NullNamespace.Default;
		
		protected List _buffer = new List();
		
		protected List _innerBuffer = new List();
		
		public NameResolutionService(CompilerContext context)
		{
			if (null == context) throw new ArgumentNullException("context");
			_context = context;
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
			_buffer.Clear();
			Resolve(_buffer, name, flags);
			return GetEntityFromBuffer();
		}
		
		public bool Resolve(List targetList, string name)
		{
			return Resolve(targetList, name, EntityType.Any);
		}
		
		public bool Resolve(List targetList, string name, EntityType flags)
		{
			IEntity entity = _context.TypeSystemServices.ResolvePrimitive(name);
			if (null != entity)
			{
				targetList.Add(entity);
				return true;
			}

			INamespace ns = _current;
			while (null != ns)
			{
				if (ns.Resolve(targetList, name, flags))
				{
					return true;
				}
				ns = ns.ParentNamespace;
			}
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
			_buffer.Clear();
			if (!ns.Resolve(_buffer, name, EntityType.Method | EntityType.Property)) return null;

			Predicate<object> notExtensionPredicate = delegate(object item)
			{
				return !IsExtensionOf(type, item as IExtensionEnabled);
			};

			_buffer.RemoveAll(notExtensionPredicate);
			return GetEntityFromBuffer();
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

			List<IGenericParameter> genericParameters = new List<IGenericParameter>(GenericsServices.FindGenericParameters(extensionType));
			if (genericParameters.Count == 0) return false;

			TypeInferrer inferrer = new TypeInferrer(genericParameters);
			inferrer.Infer(extensionType, actualType);
			return inferrer.FinalizeInference();
		}

		public IEntity ResolveQualifiedName(string name)
		{
			_buffer.Clear();
			ResolveQualifiedName(_buffer, name);
			return GetEntityFromBuffer();
		}
		
		public bool ResolveQualifiedName(List targetList, string name)
		{
			return ResolveQualifiedName(targetList, name, EntityType.Any);
		}
		
		public bool ResolveQualifiedName(List targetList, string name, EntityType flags)
		{
			if (!IsQualifiedName(name))
			{
				return Resolve(targetList, name, flags);
			}
			
			string[] parts = name.Split(DotArray);
			string topLevel = parts[0];
			
			_innerBuffer.Clear();
			if (Resolve(_innerBuffer, topLevel) && 1 == _innerBuffer.Count)
			{
				INamespace ns = _innerBuffer[0] as INamespace;
				if (null != ns)
				{
					int last = parts.Length-1;
					for (int i=1; i<last; ++i)				
					{	
						_innerBuffer.Clear();
						if (!ns.Resolve(_innerBuffer, parts[i], EntityType.Any) ||
							1 != _innerBuffer.Count)
						{
							return false;
						}				
						ns = _innerBuffer[0] as INamespace;
						if (null == ns)
						{
							return false;
						}
					}
					return ns.Resolve(targetList, parts[last], flags);
				}
			}
			return false;
		}

		public void ResolveTypeReference(TypeReference node)
		{
			if (null != node.Entity) return;

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
				node.Entity = _context.TypeSystemServices.GetArrayType(elementType, rank);
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
				node.Entity = NameNotType(node);
				return;
			}
			if (EntityType.Type != entity.EntityType)
			{
				if (EntityType.Ambiguous == entity.EntityType)
				{
					entity = AmbiguousReference(node, (Ambiguous)entity);
				}
				else
				{
					entity = NameNotType(node);
				}
			}
			else
			{
				GenericTypeReference gtr = node as GenericTypeReference;
				if (null != gtr)
				{
					ResolveTypeReferenceCollection(gtr.GenericArguments);
					entity = ResolveGenericTypeReference(gtr, entity);
				}
				
				GenericTypeDefinitionReference gtdr = node as GenericTypeDefinitionReference;
				if (null != gtdr)
				{
					IType type = (IType)entity;
					if (gtdr.GenericPlaceholders != type.GenericInfo.GenericParameters.Length)
					{
						entity = GenericArgumentsCountMismatch(gtdr, type);
					}
				}

				node.Name = entity.FullName;
			}
			
			node.Entity = entity;		
		}

		private IEntity ResolveTypeName(SimpleTypeReference node)
		{	
			_buffer.Clear();
			if (IsQualifiedName(node.Name))
			{
				ResolveQualifiedName(_buffer, node.Name);
			}
			else
			{
				Resolve(_buffer, node.Name, EntityType.Type);
			}


			// Remove from the buffer types that do not match requested generity
			FilterGenericTypes(_buffer, node);
			return GetEntityFromBuffer();
		}

		public IType ResolveGenericTypeReference(GenericTypeReference gtr, IEntity definition)
		{
			ResolveTypeReferenceCollection(gtr.GenericArguments);
			IType[] typeArguments = GetTypes(gtr.GenericArguments);
			
			return (IType)_context.GetService<GenericsServices>().ConstructEntity(
				gtr, definition, typeArguments);
		}

		public IEntity ResolveGenericReferenceExpression(GenericReferenceExpression gre, IEntity definition)
		{
			ResolveTypeReferenceCollection(gre.GenericArguments);
			IType[] typeArguments = GetTypes(gre.GenericArguments);
			
			return _context.GetService<GenericsServices>().ConstructEntity(
				gre, definition, typeArguments);
		}

		private IType[] GetTypes(TypeReferenceCollection typeReferences)
		{
			return Array.ConvertAll<TypeReference, IType>(
				typeReferences.ToArray(),
				TypeSystemServices.GetType);
		}

		private void FilterGenericTypes(List types, SimpleTypeReference node)		
		{			
			bool genericRequested = (node is GenericTypeReference || node is GenericTypeDefinitionReference);
			
			for (int i = 0; i < types.Count; i++)
			{
				IType type = types[i] as IType;
				if (type == null) continue;
				
				// Remove type from list of matches if it doesn't match requested generity
				if (type.GenericInfo != null ^ genericRequested)
				{
					types.RemoveAt(i);
					i--;
				}
			}
		}

		private IEntity NameNotType(SimpleTypeReference node)
		{
			string suggestion = GetMostSimilarTypeName(node.Name);
			_context.Errors.Add(CompilerErrorFactory.NameNotType(node, node.ToCodeString(), suggestion));
			return TypeSystemServices.ErrorEntity;
		}

		private IEntity AmbiguousReference(SimpleTypeReference node, Ambiguous entity)
		{
			_context.Errors.Add(CompilerErrorFactory.AmbiguousReference(node, node.Name, entity.Entities));
			return TypeSystemServices.ErrorEntity;
		}
		
		private IEntity GenericArgumentsCountMismatch(TypeReference node, IType type)
		{
			_context.Errors.Add(CompilerErrorFactory.GenericDefinitionArgumentCount(node, type.FullName, type.GenericInfo.GenericParameters.Length));
			return TypeSystemServices.ErrorEntity; 
		}
		
		public static IField ResolveField(IType type, string name)
		{
			return (IField)ResolveMember(type, name, EntityType.Field);
		}
		
		public static IMethod ResolveMethod(IType type, string name)
		{
			return (IMethod)ResolveMember(type, name, EntityType.Method);
		}
		
		public static IProperty ResolveProperty(IType type, string name)
		{
			return (IProperty)ResolveMember(type, name, EntityType.Property);
		}
		
		public static IEntity ResolveMember(IType type, string name, EntityType elementType)
		{
			foreach (IEntity member in type.GetMembers())
			{				
				if (elementType == member.EntityType && name == member.Name)
				{
					return member;
				}
			}
			return null;
		}
		
		public IEntity Resolve(INamespace ns, string name, EntityType elementType)
		{
			_buffer.Clear();
			ns.Resolve(_buffer, name, elementType);
			return GetEntityFromList(_buffer);
		}
		
		public IEntity Resolve(INamespace ns, string name)
		{
			return Resolve(ns, name, EntityType.Any);
		}

		IEntity GetEntityFromBuffer()
		{
			return GetEntityFromList(_buffer);
		}
		
		public static IEntity GetEntityFromList(IList list)
		{
			IEntity element = null;
			if (list.Count > 0)
			{
				if (list.Count > 1)
				{
					element = new Ambiguous(list);
				}
				else
				{
					element = (IEntity)list[0];
				}
				list.Clear();
			}
			return element;
		}
		
		static bool IsQualifiedName(string name)
		{
			return name.IndexOf('.') > 0;
		}	
		
		public static bool IsFlagSet(EntityType flags, EntityType flag)
		{
			return flag == (flags & flag);
		}
		
		public void OrganizeAssemblyTypes(Assembly asm)
		{
			CatalogPublicTypes(asm.GetTypes());
		}

		private void CatalogPublicTypes(Type[] types)
		{
			string lastNs = "!!not a namespace!!";
			NamespaceEntity lastNsEntity = null;

			foreach (Type type in types)
			{
				if (!type.IsPublic) continue;

				string ns = type.Namespace ?? string.Empty;
				//retrieve the namespace only if we don't have it handy already
				//usually we'll have it since GetExportedTypes() seems to export
				//types in a sorted fashion.
				if (ns != lastNs)
				{
					lastNs = ns;
					lastNsEntity = GetNamespace(ns);
					lastNsEntity.Add(type);
				}
				else
				{
					lastNsEntity.Add(type);
				}
			}
		}
		public NamespaceEntity GetNamespace(string ns)
		{
			string[] namespaceHierarchy = ns.Split('.');
			string topLevelName = namespaceHierarchy[0];
			NamespaceEntity topLevel = GetTopLevelNamespace(topLevelName);
			NamespaceEntity current = topLevel;
			for (int i=1; i<namespaceHierarchy.Length; ++i)
			{
				current = current.GetChildNamespace(namespaceHierarchy[i]);
			}
			return current;	
		}
		
		NamespaceEntity GetTopLevelNamespace(string topLevelName)
		{
			GlobalNamespace globalNS = GetGlobalNamespace();
			if (globalNS == null) return null;
			
			NamespaceEntity entity = (NamespaceEntity)globalNS.GetChild(topLevelName);
			if (null == entity)
			{
				entity = new NamespaceEntity(null, _context.TypeSystemServices, topLevelName);
				globalNS.SetChild(topLevelName, entity);
			}
			return entity;
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

		private static void FlattenChildNamespaces(List<INamespace> list, INamespace ns)
		{
			foreach (IEntity ent in ns.GetMembers())
			{
				if (EntityType.Namespace != ent.EntityType) continue;
				list.Add((INamespace) ent);
				FlattenChildNamespaces(list, (INamespace) ent);
			}
		}

		public string GetMostSimilarTypeName(string name)
		{
			string[] nsHierarchy = name.Split('.');
			int nshLen = nsHierarchy.Length;
			string suggestion = null;

			if (nshLen > 1)
			{
				INamespace ns = null;
				INamespace prevNs = null;
				for (int i = 1; i < nshLen; i++)
				{
					string currentNsName = string.Join(".", nsHierarchy, 0, i);
					ns = ResolveQualifiedName(currentNsName) as INamespace;
					if (null == ns)
					{
						nsHierarchy[i-1] = GetMostSimilarMemberName(prevNs, nsHierarchy[i-1], EntityType.Namespace);
						if (null == nsHierarchy[i-1]) break;
						i--; continue; //reloop to resolve step
					}
					prevNs = ns;
				}
				suggestion = GetMostSimilarMemberName(ns, nsHierarchy[nshLen-1], EntityType.Type);
				if (null != suggestion)
				{
					nsHierarchy[nshLen-1] = suggestion;
					return string.Join(".", nsHierarchy);
				}
			}
		
			List<INamespace> nsList = new List<INamespace>();
			FlattenChildNamespaces(nsList, GetGlobalNamespace());
			nsList.Reverse();//most recently added namespaces first
			foreach (INamespace nse in nsList)
			{
				suggestion = GetMostSimilarMemberName(nse, nsHierarchy[nshLen-1], EntityType.Type);
				if (null != suggestion) return nse.ToString()+"."+suggestion;
			}
			return GetMostSimilarMemberName(GetGlobalNamespace(), nsHierarchy[nshLen-1], EntityType.Type);
		}

		public string GetMostSimilarMemberName(INamespace ns, string name, EntityType elementType)
		{
			if (null == ns) return null;

			string expectedSoundex = ToSoundex(name);
			string lastMemberName = null;
			foreach (IEntity member in ns.GetMembers())
			{
				if (EntityType.Any != elementType && elementType != member.EntityType)
					continue;
				if (lastMemberName == member.Name)
					continue;//no need to check this name again
				//TODO: try Levenshtein distance or Metaphone instead of Soundex.
				if (expectedSoundex == ToSoundex(member.Name))
				{
					return member.Name;
				}
				lastMemberName = member.Name;
			}
			return null;
		}

		private static string ToSoundex(string s)
		{
			if (s.Length < 2) return null;
			char[] code = "?0000".ToCharArray();
			string ws = s.ToLowerInvariant();
			int wsLen = ws.Length;
			char lastChar = ' ';
			int lastCharPos = 1;

			code[0] = ws[0];
			for (int i = 1; i < wsLen; i++)
			{
				char wsc = ws[i];
				char c = ' ';
				if (wsc == 'b' || wsc == 'f' || wsc == 'p' || wsc == 'v') c = '1';
				if (wsc == 'c' || wsc == 'g' || wsc == 'j' || wsc == 'k' || wsc == 'q' || wsc == 's' || wsc == 'x' || wsc == 'z') c = '2';
				if (wsc == 'd' || wsc == 't') c = '3';
				if (wsc == 'l') c = '4';
				if (wsc == 'm' || wsc == 'n') c = '5';
				if (wsc == 'r') c = '6';
				if (c == lastChar) continue;
				lastChar = c;
				if (c == ' ') continue;
				code[lastCharPos] = c;
				lastCharPos++;
				if (lastCharPos > 4) break;
			}
			return new string(code);
        }

	}
}
