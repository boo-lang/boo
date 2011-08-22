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
using System.Reflection;
using Boo.Lang.Compiler.Util;

namespace Boo.Lang.Compiler.TypeSystem.Reflection
{
	class AssemblyReference : IAssemblyReference, IEquatable<AssemblyReference>
	{
		private readonly Assembly _assembly;
		private readonly ReflectionTypeSystemProvider _provider;
		private INamespace _rootNamespace;

		private readonly MemoizedFunction<Type, IType> _typeEntityCache;
		private readonly MemoizedFunction<MemberInfo, IEntity> _memberCache;

		internal AssemblyReference(ReflectionTypeSystemProvider provider, Assembly assembly)
		{
			if (null == assembly)
				throw new ArgumentNullException("assembly");
			_provider = provider;
			_assembly = assembly;
			_typeEntityCache = new MemoizedFunction<Type, IType>(NewType);
			_memberCache = new MemoizedFunction<MemberInfo, IEntity>(NewEntityForMember);
		}
		
		public string Name
		{
			get { return _name ?? (_name = new AssemblyName(_assembly.FullName).Name); }
		}

		string _name;

		public string FullName
		{
			get { return _assembly.FullName; }
		}
		
		public EntityType EntityType
		{
			get { return EntityType.Assembly; }
		}
		
		public Assembly Assembly
		{
			get { return _assembly; }
		}

		#region Implementation of ICompileUnit

		public INamespace RootNamespace
		{
			get
			{
				if (_rootNamespace != null)
					return _rootNamespace;
				return _rootNamespace = CreateRootNamespace();
			}
		}

		private INamespace CreateRootNamespace()
		{
			return new ReflectionNamespaceBuilder(_provider, _assembly).Build();
		}

		#endregion

		public override bool Equals(object other)
		{
			if (null == other) return false;
			if (this == other) return true;

			AssemblyReference aref = other as AssemblyReference;
			return Equals(aref);
		}

		public bool Equals(AssemblyReference other)
		{
			if (null == other) return false;
			if (this == other) return true;

			return IsReferencing(other._assembly);
		}

		private bool IsReferencing(Assembly assembly)
		{
			return AssemblyEqualityComparer.Default.Equals(_assembly, assembly);
		}

		public override int GetHashCode()
		{
			return AssemblyEqualityComparer.Default.GetHashCode(_assembly);
		}

		public override string ToString()
		{
			return _assembly.FullName;
		}

		public IType Map(Type type)
		{
			AssertAssembly(type);
			return _typeEntityCache.Invoke(type);
		}

		public IEntity MapMember(MemberInfo mi)
		{
			AssertAssembly(mi);
			if (mi.MemberType == MemberTypes.NestedType)
				return Map((Type)mi);
			return _memberCache.Invoke(mi);
		}

		private void AssertAssembly(MemberInfo member)
		{
			if (!IsReferencing(member.Module.Assembly))
				throw new ArgumentException(string.Format("{0} doesn't belong to assembly '{1}'.", member, _assembly));
		}

		private IEntity NewEntityForMember(MemberInfo mi)
		{
			switch (mi.MemberType)
			{
				case MemberTypes.Method:
					return new ExternalMethod(_provider, (MethodBase)mi);
				case MemberTypes.Constructor:
					return new ExternalConstructor(_provider, (ConstructorInfo)mi);
				case MemberTypes.Field:
					return new ExternalField(_provider, (FieldInfo)mi);
				case MemberTypes.Property:
					return new ExternalProperty(_provider, (PropertyInfo)mi);
				case MemberTypes.Event:
					return new ExternalEvent(_provider, (EventInfo)mi);
				default:
					throw new NotImplementedException(mi.ToString());
			}
		}

		public void MapTo(Type type, IType entity)
		{
			AssertAssembly(type);
			_typeEntityCache.Add(type, entity);
		}

		private IType NewType(Type type)
		{
			return type.IsArray
				? Map(type.GetElementType()).MakeArrayType(type.GetArrayRank())
				: CreateEntityForType(type);
		}

		private IType CreateEntityForType(Type type)
		{
			if (type.IsGenericParameter) return new ExternalGenericParameter(_provider, type);
			if (type.IsSubclassOf(Types.MulticastDelegate)) return _provider.CreateEntityForCallableType(type);
			return _provider.CreateEntityForRegularType(type);
		}

	}
}
