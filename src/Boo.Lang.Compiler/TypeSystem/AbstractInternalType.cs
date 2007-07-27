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

	public abstract class AbstractInternalType : IInternalEntity, IType, INamespace
	{
		public static readonly IConstructor[] NoConstructors = new IConstructor[0];

		protected TypeSystemServices _typeSystemServices;

		protected TypeDefinition _typeDefinition;

		protected INamespace _parentNamespace;

		protected InternalGenericTypeInfo _genericInfo = null;

		private string _fullName = null;

		protected AbstractInternalType(TypeSystemServices typeSystemServices, TypeDefinition typeDefinition)
		{
			_typeSystemServices = typeSystemServices;
			_typeDefinition = typeDefinition;
			
			if (typeDefinition.GenericParameters.Count > 0)
			{
				_genericInfo = new InternalGenericTypeInfo(typeSystemServices, this, typeDefinition.GenericParameters);
			}
		}

		protected virtual string BuildFullName()
		{
			string fullName = _typeDefinition.FullName;

			if (_genericInfo != null)
			{
				fullName = string.Format("{0}`{1}", fullName, _genericInfo.GenericParameters.Length);
			}

			return fullName;
		}

		public string FullName
		{
			get { return _fullName ?? (_fullName = BuildFullName());  }
		}

		public string Name
		{
			get
			{
				return _typeDefinition.Name;
			}
		}

		public Node Node
		{
			get
			{
				return _typeDefinition;
			}
		}

		public IType NestingType
		{
			get
			{
				return _typeDefinition.ParentNode.Entity as IType;
			}
		}

		public virtual INamespace ParentNamespace
		{
			get
			{
				if (null == _parentNamespace)
				{
					_parentNamespace = (INamespace)TypeSystemServices.GetEntity(_typeDefinition.ParentNode);
				}
				return _parentNamespace;
			}
		}

		public virtual bool Resolve(List targetList, string name, EntityType flags)
		{
			bool found = false;

			// Try to resolve name as a generic parameter if applicable
			if (_genericInfo != null && NameResolutionService.IsFlagSet(flags, EntityType.Type))
			{
				IType genericParameter = _genericInfo.GetGenericParameter(name);
				if (genericParameter != null)
				{
					targetList.AddUnique(genericParameter);
					found = true;
				}
			}

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
				return _typeDefinition;
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
				return NodeType.ClassDefinition == _typeDefinition.NodeType;
			}
		}

		public bool IsAbstract
		{
			get
			{
				return _typeDefinition.IsAbstract;
			}
		}

		virtual public bool IsFinal
		{
			get
			{
				return _typeDefinition.IsFinal || IsValueType;
			}
		}

		public bool IsInterface
		{
			get
			{
				return NodeType.InterfaceDefinition == _typeDefinition.NodeType;
			}
		}

		public bool IsEnum
		{
			get
			{
				return NodeType.EnumDefinition == _typeDefinition.NodeType;
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
			foreach (Attribute attribute in _typeDefinition.Attributes)
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
			if (_typeDefinition.BaseTypes.Count > 0)
			{
				List buffer = new List();

				foreach (TypeReference baseType in _typeDefinition.BaseTypes)
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

		public virtual EntityType EntityType
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
			List buffer = new List(_typeDefinition.BaseTypes.Count);
			foreach (TypeReference baseType in _typeDefinition.BaseTypes)
			{
				IType type = TypeSystemServices.GetType(baseType);
				if (type.IsInterface) buffer.AddUnique(type);
			}
			return (IType[])buffer.ToArray(new IType[buffer.Count]);
		}

		public virtual IEntity[] GetMembers()
		{
			return GetMemberEntities(_typeDefinition.Members);
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

		IGenericTypeInfo IType.GenericInfo
		{
			get { return _genericInfo; }
		}

		IConstructedTypeInfo IType.ConstructedInfo
		{
			get { return null; }
		}
	}

}
