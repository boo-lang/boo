#region license
// Copyright (c) 2003, 2004, 2005 Rodrigo B. de Oliveira (rbo@acm.org)
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
using System.Reflection;
using Boo.Lang.Compiler.Util;

namespace Boo.Lang.Compiler.TypeSystem.Reflection
{
	public class ReflectionTypeSystemProvider : IReflectionTypeSystemProvider
	{
		private readonly Memo<Assembly, AssemblyReference> _referenceCache = new Memo<Assembly, AssemblyReference>(AssemblyEqualityComparer.Default);
		private readonly Memo<Type, IType> _typeEntityCache = new Memo<Type, IType>();
		private readonly Memo<MemberInfo, IEntity> _memberCache = new Memo<MemberInfo, IEntity>();

		public ReflectionTypeSystemProvider()
		{
			MapTo(typeof(Builtins.duck), new DuckTypeImpl(this));
			MapTo(typeof(void), new VoidTypeImpl(this));
		}

		protected void MapTo(Type type, IType entity)
		{
			_typeEntityCache.Add(type, entity);
		}

		private ReflectionTypeSystemProvider(Memo<MemberInfo, IEntity> memberCache,
			Memo<Assembly, AssemblyReference> referenceCache,
			Memo<Type, IType> typeEntityCache)
		{
			_memberCache = memberCache;
			_referenceCache = referenceCache;
			_typeEntityCache = typeEntityCache;
		}

		#region Implementation of ICompilerReferenceProvider

		public ICompileUnit ForAssembly(Assembly assembly)
		{
			return _referenceCache.Produce(assembly, CreateReference);
		}

		private AssemblyReference CreateReference(Assembly assembly)
		{
			return new AssemblyReference(this, assembly);
		}

		#endregion

		#region Implementation of IReflectionTypeSystemProvider

		public IType Map(Type type)
		{
			return _typeEntityCache.Produce(type, NewType);
		}

		public IMethod Map(MethodInfo method)
		{
			return (IMethod) MapMember(method);
		}

		public IConstructor Map(ConstructorInfo ctor)
		{
			return (IConstructor) MapMember(ctor);
		}

		public IEntity Map(MemberInfo mi)
		{
			return MapMember(mi);
		}

		private IEntity MapMember(MemberInfo mi)
		{
			if (mi.MemberType == MemberTypes.NestedType)
				return Map((Type) mi);

			return _memberCache.Produce(mi, NewEntityForMember);
		}

		private IEntity NewEntityForMember(MemberInfo mi)
		{
			switch (mi.MemberType)
			{
				case MemberTypes.Method:
					return new ExternalMethod(this, (MethodBase) mi);
				case MemberTypes.Constructor:
					return new ExternalConstructor(this, (ConstructorInfo) mi);
				case MemberTypes.Field:
					return new ExternalField(this, (FieldInfo)mi);
				case MemberTypes.Property:
					return new ExternalProperty(this, (PropertyInfo)mi);
				case MemberTypes.Event:
					return new ExternalEvent(this, (EventInfo)mi);
				default:
					throw new NotImplementedException(mi.ToString());
			}
		}

		public IParameter[] Map(ParameterInfo[] parameters)
		{
			IParameter[] mapped = new IParameter[parameters.Length];
			for (int i = 0; i < parameters.Length; ++i)
			{
				mapped[i] = new ExternalParameter(this, parameters[i]);
			}
			return mapped;
		}

		public virtual IReflectionTypeSystemProvider Clone()
		{
			return new ReflectionTypeSystemProvider(_memberCache.Clone(), _referenceCache.Clone(), _typeEntityCache.Clone());
		}

		public IEntity Map(MemberInfo[] members)
		{
			switch (members.Length)
			{
				case 0:
					return null;
				case 1:
					return Map(members[0]);
				default:
					IEntity[] entities = new IEntity[members.Length];
					for (int i = 0; i < entities.Length; ++i)
						entities[i] = Map(members[i]);
					return new Ambiguous(entities);
			}
		}

		private IType NewType(Type type)
		{
			return type.IsArray
				? Map(type.GetElementType()).MakeArrayType(type.GetArrayRank())
				: CreateEntityForType(type);
		}

		private IType CreateEntityForType(Type type)
		{
			if (type.IsGenericParameter) return new ExternalGenericParameter(this, type);
			if (type.IsSubclassOf(Types.MulticastDelegate)) return CreateEntityForCallableType(type);
			return CreateEntityForRegularType(type);
		}

		protected virtual IType CreateEntityForRegularType(Type type)
		{
			return new ExternalType(this, type);
		}

		protected virtual IType CreateEntityForCallableType(Type type)
		{
			return new ExternalCallableType(this, type);
		}

		#endregion

		sealed class DuckTypeImpl : ExternalType
		{
			internal DuckTypeImpl(IReflectionTypeSystemProvider provider)
				: base(provider, Types.Object)
			{
			}
		}

		#region VoidTypeImpl
		sealed class VoidTypeImpl : ExternalType
		{
			internal VoidTypeImpl(IReflectionTypeSystemProvider provider)
				: base(provider, Types.Void)
			{
			}

			override public bool Resolve(ICollection<IEntity> resultingSet, string name, EntityType typesToConsider)
			{
				return false;
			}

			override public bool IsSubclassOf(IType other)
			{
				return false;
			}

			override public bool IsAssignableFrom(IType other)
			{
				return false;
			}
		}

		#endregion

	}
}
