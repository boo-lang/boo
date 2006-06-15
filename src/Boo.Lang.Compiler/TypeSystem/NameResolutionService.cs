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
			if (null == context)
			{
				throw new ArgumentNullException("context");
			}
			_context = context;
		}
		
		public INamespace GlobalNamespace
		{
			get
			{
				return _global;
			}
			
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
			if (null == ns)
			{
				throw new ArgumentNullException("ns");
			}
			_current = ns;
		}
		
		public INamespace CurrentNamespace
		{
			get
			{
				return _current;
			}
		}
		
		public void Reset()
		{
			EnterNamespace(_global);
		}
		
		public void Restore(INamespace saved)
		{
			if (null == saved)
			{
				throw new ArgumentNullException("saved");
			}
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
			else
			{
				INamespace ns = _current;
				while (null != ns)
				{					
					if (ns.Resolve(targetList, name, flags))
					{
						return true;
					}
					ns = ns.ParentNamespace;
				}
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

		class IsNotExtensionOf
		{
			private IType _type;

			public IsNotExtensionOf(IType type)
			{
				_type = type;
			}

			public bool Match(object item)
			{
				IExtensionEnabled e = item as IExtensionEnabled;
				if (e == null) return true;
				if (!e.IsExtension) return true;
				IParameter[] parameters = e.GetParameters();
				if (parameters.Length == 0) return true;
				return !parameters[0].Type.IsAssignableFrom(_type);
			}
		}

		private IEntity ResolveExtensionForType(INamespace ns, IType type, string name)
		{
			_buffer.Clear();
			if (!ns.Resolve(_buffer, name, EntityType.Method|EntityType.Property)) return null;
			_buffer.RemoveAll(new Predicate(new IsNotExtensionOf(type).Match));
			return GetEntityFromBuffer();
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
			if (NodeType.ArrayTypeReference == node.NodeType)
			{
				ResolveArrayTypeReference((ArrayTypeReference)node);
			}
			else
			{
				ResolveSimpleTypeReference((SimpleTypeReference)node);
			}
		}
		
		public void ResolveArrayTypeReference(ArrayTypeReference node)
		{
			if (node.Entity != null)
			{
				return;
			}

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
		
		public void ResolveSimpleTypeReference(SimpleTypeReference node)
		{
			if (null != node.Entity)
			{
				return;
			}
			
			IEntity info = null;
			if (IsQualifiedName(node.Name))
			{
				info = ResolveQualifiedName(node.Name);
			}
			else
			{
				info = Resolve(node.Name, EntityType.Type);
			}
			
			if (null == info)
			{
				info = NameNotType(node);
			}
			else
			{
				if (EntityType.Type != info.EntityType)
				{
					if (EntityType.Ambiguous == info.EntityType)
					{
						info = AmbiguousReference(node, (Ambiguous)info);
					}
					else
					{
						info = NameNotType(node);
					}
				}
				else
				{
					node.Name = info.FullName;
				}
			}
			
			node.Entity = info;
		}
		
		private IEntity NameNotType(SimpleTypeReference node)
		{
			_context.Errors.Add(CompilerErrorFactory.NameNotType(node, node.Name));
			return TypeSystemServices.ErrorEntity;
		}
		
		private IEntity AmbiguousReference(SimpleTypeReference node, Ambiguous entity)
		{
			_context.Errors.Add(CompilerErrorFactory.AmbiguousReference(node, node.Name, entity.Entities));
			return TypeSystemServices.ErrorEntity;
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
			Type[] types = asm.GetTypes();
			foreach (Type type in types)
			{
				if (type.IsPublic)
				{
					string ns = type.Namespace;
					if (null == ns)
					{
						ns = string.Empty;
					}
				
					GetNamespace(ns).Add(type);
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
		
	}
}
