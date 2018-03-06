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
using Boo.Lang.Compiler.TypeSystem.Services;
using Boo.Lang.Compiler.Util;
using Boo.Lang.Environments;

namespace Boo.Lang.Compiler.TypeSystem.Internal
{
	public abstract class AbstractInternalType : InternalEntity<TypeDefinition>, IType, IGenericTypeInfo
	{
		protected readonly InternalTypeSystemProvider _provider;

		private readonly Dictionary<IType[], IType> _constructedTypes = new Dictionary<IType[], IType>(ArrayEqualityComparer<IType>.Default);

		private readonly System.Collections.Generic.List<IEntity> _memberEntitiesCache = new System.Collections.Generic.List<IEntity>();

		protected AbstractInternalType(InternalTypeSystemProvider provider, TypeDefinition typeDefinition) : base(typeDefinition)
		{
			_provider = provider;
			typeDefinition.Members.Changed += (sender, args) => ClearMemberEntitiesCache();
		}

		override public string FullName
		{
			get { return _node.FullName; }
		}

		public IEntity DeclaringEntity
		{
			get { return _node.ParentNode.Entity as IType; }
		}

		public virtual INamespace ParentNamespace
		{
			get { return (INamespace)TypeSystemServices.GetEntity(_node.ParentNode); }
		}

		public virtual bool Resolve(ICollection<IEntity> resultingSet, string name, EntityType typesToConsider)
		{
			if (ResolveGenericParameter(resultingSet, name, typesToConsider)) return true;

			return ResolveMember(resultingSet, name, typesToConsider);
		}

		protected bool ResolveMember(ICollection<IEntity> resolvedSet, string name, EntityType typesToConsider)
		{
			return My<NameResolutionService>.Instance.Resolve(name, GetMembers(), typesToConsider, resolvedSet);
		}

		protected bool ResolveGenericParameter(ICollection<IEntity> targetList, string name, EntityType flags)
		{
			if (!Entities.IsFlagSet(flags, EntityType.Type))
				return false;

			foreach (var p in _node.GenericParameters.Where(gpd => gpd.Name == name))
			{
				targetList.Add(p.Entity);
				return true;
			}
			return false;
		}

		public virtual IType BaseType
		{
			get { return null; }
		}

		public TypeDefinition TypeDefinition
		{
			get { return _node; }
		}

		public IType Type
		{
			get { return this; }
		}

		public bool IsByRef { get; protected set; }

		protected IType _elementType;

		public IType ElementType
		{
			get { return _elementType ?? (_elementType = CreateElementType()); }
		}

		protected virtual IType CreateElementType()
		{
			return null;
		}

		public bool IsClass
		{
			get { return NodeType.ClassDefinition == _node.NodeType; }
		}

		public bool IsAbstract
		{
			get { return _node.IsAbstract; }
		}

		virtual public bool IsFinal
		{
			get { return _node.IsFinal || IsValueType; }
		}

		public bool IsInterface
		{
			get { return NodeType.InterfaceDefinition == _node.NodeType; }
		}

		public bool IsEnum
		{
			get { return NodeType.EnumDefinition == _node.NodeType; }
		}

		virtual public bool IsValueType
		{
			get { return false; }
		}

		public bool IsArray
		{
			get { return false; }
		}

		virtual public bool IsPointer
		{
			get { return false; }
		}

		public virtual bool IsVoid
		{
			get { return false; }
		}

		public virtual int GetTypeDepth()
		{
			return 1;
		}

		public IEntity GetDefaultMember()
		{
			IType defaultMemberAttribute = My<TypeSystemServices>.Instance.Map(Types.DefaultMemberAttribute);
			foreach (var attribute in _node.Attributes)
			{
				var ctor = TypeSystemServices.GetEntity(attribute) as IConstructor;
				if (null != ctor && defaultMemberAttribute == ctor.DeclaringType)
				{
					var memberName = attribute.Arguments[0] as StringLiteralExpression;
					if (null != memberName)
					{
						var buffer = new System.Collections.Generic.List<IEntity>();
						Resolve(buffer, memberName.Value, EntityType.Any);
						return Entities.EntityFromList(buffer);
					}
				}
			}
			return null;
		}

		override public EntityType EntityType
		{
			get { return EntityType.Type; }
		}

		public virtual bool IsSubclassOf(IType other)
		{
			return false;
		}

		public virtual bool IsAssignableFrom(IType other)
		{
			return this == other
				|| (!IsValueType && other.IsNull())
				|| other.IsSubclassOf(this);
		}

		public IType[] GetInterfaces()
		{
			var buffer = new List<IType>(_node.BaseTypes.Count);
			foreach (TypeReference baseType in _node.BaseTypes)
			{
				var type = TypeSystemServices.GetType(baseType);
				if (type.IsInterface) buffer.AddUnique(type);
			}
			return buffer.ToArray();
		}

		public virtual IEnumerable<IEntity> GetMembers()
		{
			if (_memberEntitiesCache.Count > 0)
				return _memberEntitiesCache;
			GetMemberEntities(_node.Members);
			return _memberEntitiesCache;
		}

		private void ClearMemberEntitiesCache()
		{
			_memberEntitiesCache.Clear();
		}

		private void GetMemberEntities(TypeMemberCollection members)
		{
			foreach (TypeMember member in members.Except<StatementTypeMember,Destructor>())
				_memberEntitiesCache.Add(_provider.EntityFor(member));
		}

		override sealed public string ToString()
		{
			return this.DisplayName();
		}

		public IGenericTypeInfo GenericInfo
		{
			get { return TypeDefinition.GenericParameters.Count != 0 ? this : null; }
		}

		public IConstructedTypeInfo ConstructedInfo
		{
			get { return null; }
		}

		IGenericParameter[] IGenericParametersProvider.GenericParameters
		{
			get { return Array.ConvertAll(_node.GenericParameters.ToArray(), gpd => (IGenericParameter) gpd.Entity); }
		}

		IType IGenericTypeInfo.ConstructType(IType[] arguments)
		{
			IType constructed;
			if (!_constructedTypes.TryGetValue(arguments, out constructed))
			{
				constructed = CreateConstructedType(arguments);
				_constructedTypes.Add(arguments, constructed);
			}

			return constructed;
		}

		protected virtual IType CreateConstructedType(IType[] arguments)
		{
			return new GenericConstructedType(this, arguments);
		}

		private ArrayTypeCache _arrayTypes;

		public IArrayType MakeArrayType(int rank)
		{
			if (null == _arrayTypes)
				_arrayTypes = new ArrayTypeCache(this);
			return _arrayTypes.MakeArrayType(rank);
		}

		virtual public IType MakePointerType()
		{
			return null;
		}

		public bool IsGenericType
		{
			get { return _node.GenericParameters.Count > 0; }
		}

		public IType GenericDefinition
		{
			get { return this.IsGenericType ? this : null; }
		}
	}
}
