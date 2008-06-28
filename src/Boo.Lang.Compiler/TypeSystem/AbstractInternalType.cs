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
	using System.Reflection;
	using Boo.Lang.Compiler.Ast;
	using Attribute = Boo.Lang.Compiler.Ast.Attribute;
	using System.Collections.Generic;

	public abstract class AbstractInternalType : InternalEntity<TypeDefinition>, IType, INamespace, IGenericTypeInfo
	{
		public static readonly IConstructor[] NoConstructors = new IConstructor[0];

		protected TypeSystemServices _typeSystemServices;

		protected INamespace _parentNamespace;

		private string _fullName = null;

		private IGenericParameter[] _genericParameters = null;

		private Dictionary<IType[], IType> _constructedTypes = new Dictionary<IType[], IType>(ArrayEqualityComparer<IType>.Default);

		protected AbstractInternalType(TypeSystemServices typeSystemServices, TypeDefinition typeDefinition) : base(typeDefinition)
		{
			_typeSystemServices = typeSystemServices;
		}

		protected virtual string BuildFullName()
		{
			return _node.FullName;
		}

		override public string FullName
		{
			get { return _fullName ?? (_fullName = BuildFullName());  }
		}

		public IType NestingType
		{
			get { return _node.ParentNode.Entity as IType; }
		}

		public virtual INamespace ParentNamespace
		{
			get
			{
				if (null == _parentNamespace)
				{
					_parentNamespace = (INamespace)TypeSystemServices.GetEntity(_node.ParentNode);
				}
				return _parentNamespace;
			}
		}

		public virtual bool Resolve(List targetList, string name, EntityType flags)
		{
			if (ResolveGenericParameter(targetList, name, flags)) return true;

			return ResolveMember(targetList, name, flags);
		}

		protected bool ResolveMember(List targetList, string name, EntityType flags)
		{
			bool found = false;

			// Try to resolve name as a member
			foreach (IEntity entity in GetMembers())
			{
				if (entity.Name == name && NameResolutionService.IsFlagSet(flags, entity.EntityType))
				{
					targetList.AddUnique(entity);
					found = true;
				}
			}

			return found;
		}

		protected bool ResolveGenericParameter(List targetList, string name, EntityType flags)
		{
			// Try to resolve name as a generic parameter
			if (NameResolutionService.IsFlagSet(flags, EntityType.Type))
			{
				foreach (GenericParameterDeclaration gpd in _node.GenericParameters)
				{
					if (gpd.Name == name)
					{
						targetList.AddUnique(gpd.Entity);
						return true;
					}
				}
			}
			return false;
		}

		public virtual IType BaseType
		{
			get
			{
				return null;
			}
		}

		public TypeDefinition TypeDefinition
		{
			get
			{
				return _node;
			}
		}

		public IType Type
		{
			get
			{
				return this;
			}
		}

		public bool IsByRef
		{
			get
			{
				return false;
			}
		}

		public IType GetElementType()
		{
			return null;
		}

		public bool IsClass
		{
			get
			{
				return NodeType.ClassDefinition == _node.NodeType;
			}
		}

		public bool IsAbstract
		{
			get
			{
				return _node.IsAbstract;
			}
		}

		virtual public bool IsFinal
		{
			get
			{
				return _node.IsFinal || IsValueType;
			}
		}

		public bool IsInterface
		{
			get
			{
				return NodeType.InterfaceDefinition == _node.NodeType;
			}
		}

		public bool IsEnum
		{
			get
			{
				return NodeType.EnumDefinition == _node.NodeType;
			}
		}

		virtual public bool IsValueType
		{
			get
			{
				return false;
			}
		}

		public bool IsArray
		{
			get
			{
				return false;
			}
		}

		public virtual int GetTypeDepth()
		{
			return 1;
		}

		public IEntity GetDefaultMember()
		{
			IType defaultMemberAttribute = _typeSystemServices.Map(Types.DefaultMemberAttribute);
			foreach (Attribute attribute in _node.Attributes)
			{
				IConstructor tag = TypeSystemServices.GetEntity(attribute) as IConstructor;
				if (null != tag)
				{
					if (defaultMemberAttribute == tag.DeclaringType)
					{
						StringLiteralExpression memberName = attribute.Arguments[0] as StringLiteralExpression;
						if (null != memberName)
						{
							List buffer = new List();
							Resolve(buffer, memberName.Value, EntityType.Any);
							return NameResolutionService.GetEntityFromList(buffer);
						}
					}
				}
			}
			if (_node.BaseTypes.Count > 0)
			{
				List buffer = new List();

				foreach (TypeReference baseType in _node.BaseTypes)
				{
					IType tag = TypeSystemServices.GetType(baseType);
					IEntity defaultMember = tag.GetDefaultMember();
					if (defaultMember != null)
					{
						if (tag.IsInterface)
						{
							buffer.AddUnique(defaultMember);
						}
						else //non-interface base class trumps interfaces
						{
							return defaultMember;
						}
					}
				}
				return NameResolutionService.GetEntityFromList(buffer);
			}
			return null;
		}

		override public EntityType EntityType
		{
			get
			{
				return EntityType.Type;
			}
		}

		public virtual bool IsSubclassOf(IType other)
		{
			return false;
		}

		public virtual bool IsAssignableFrom(IType other)
		{
			return this == other ||
					(!this.IsValueType && Null.Default == other) ||
					other.IsSubclassOf(this);
		}

		public virtual IConstructor[] GetConstructors()
		{
			return NoConstructors;
		}

		public IType[] GetInterfaces()
		{
			List buffer = new List(_node.BaseTypes.Count);
			foreach (TypeReference baseType in _node.BaseTypes)
			{
				IType type = TypeSystemServices.GetType(baseType);
				if (type.IsInterface) buffer.AddUnique(type);
			}
			return (IType[])buffer.ToArray(new IType[buffer.Count]);
		}

		public virtual IEntity[] GetMembers()
		{
			return GetMemberEntities(_node.Members);
		}

		private IEntity[] GetMemberEntities(TypeMemberCollection members)
		{
			IEntity[] entities = new IEntity[members.Count];
			for (int i = 0; i < entities.Length; ++i)
			{
				entities[i] = _typeSystemServices.GetMemberEntity(members[i]);
			}
			return entities;
		}


		override public string ToString()
		{
			return FullName;
		}

		public IGenericTypeInfo GenericInfo
		{
			get 
			{
				if (TypeDefinition.GenericParameters.Count != 0)
				{
					return this;
				}
				return null;
			}
		}

		public IConstructedTypeInfo ConstructedInfo
		{
			get { return null; }
		}

		IGenericParameter[] IGenericTypeInfo.GenericParameters
		{
			get
			{
				if (_genericParameters == null)
				{
					_genericParameters = Array.ConvertAll<GenericParameterDeclaration, IGenericParameter>(
						_node.GenericParameters.ToArray(),
						delegate(GenericParameterDeclaration gpd) { return (IGenericParameter)gpd.Entity; } );
				}

				return _genericParameters;
			}
		}

		IType IGenericTypeInfo.ConstructType(IType[] arguments)
		{
			IType constructed = null;
			if (!_constructedTypes.TryGetValue(arguments, out constructed))
			{
				constructed = CreateConstructedType(arguments);
				_constructedTypes.Add(arguments, constructed);
			}

			return constructed;
		}

		protected virtual IType CreateConstructedType(IType[] arguments)
		{
			return new GenericConstructedType(_typeSystemServices, this, arguments);
		}
	}

}
