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
		private readonly MemoizedFunction<Assembly, AssemblyReference> _referenceCache;
		
		public ReflectionTypeSystemProvider()
		{
			_referenceCache = new MemoizedFunction<Assembly, AssemblyReference>(AssemblyEqualityComparer.Default, CreateReference);
			Initialize();
		}

		virtual protected void Initialize()
		{
			MapTo(typeof(object), new ObjectTypeImpl(this));
			MapTo(typeof(Builtins.duck), new ObjectTypeImpl(this));
			MapTo(typeof(void), new VoidTypeImpl(this));
		}

		protected void MapTo(Type type, IType entity)
		{
			AssemblyReferenceFor(type.Assembly).MapTo(type, entity);
		}

		private ReflectionTypeSystemProvider(MemoizedFunction<Assembly, AssemblyReference> referenceCache)
		{
			_referenceCache = referenceCache;
		}

		#region Implementation of ICompilerReferenceProvider

		public IAssemblyReference ForAssembly(Assembly assembly)
		{
			return AssemblyReferenceFor(assembly);
		}

		private AssemblyReference AssemblyReferenceFor(Assembly assembly)
		{
			return _referenceCache.Invoke(assembly);
		}

		private AssemblyReference CreateReference(Assembly assembly)
		{
			return new AssemblyReference(this, assembly);
		}

		#endregion

		#region Implementation of IReflectionTypeSystemProvider

		public IType Map(Type type)
		{
			return AssemblyReferenceFor(type.Assembly).Map(type);
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

		public IParameter[] Map(ParameterInfo[] parameters)
		{
			var mapped = new IParameter[parameters.Length];
			for (int i = 0; i < parameters.Length; ++i)
			{
				mapped[i] = new ExternalParameter(this, parameters[i]);
			}
			return mapped;
		}

		private IEntity MapMember(MemberInfo mi)
		{
			return AssemblyReferenceFor(mi.DeclaringType.Assembly).MapMember(mi);
		}

		public virtual IReflectionTypeSystemProvider Clone()
		{
			return new ReflectionTypeSystemProvider(_referenceCache.Clone());
		}

		public virtual IType CreateEntityForRegularType(Type type)
		{
			return new ExternalType(this, type);
		}

		public virtual IType CreateEntityForCallableType(Type type)
		{
			return new ExternalCallableType(this, type);
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
					var entities = new IEntity[members.Length];
					for (int i = 0; i < entities.Length; ++i)
						entities[i] = Map(members[i]);
					return new Ambiguous(entities);
			}
		}

		#endregion

		sealed class ObjectTypeImpl : ExternalType
		{
			internal ObjectTypeImpl(IReflectionTypeSystemProvider provider)
				: base(provider, Types.Object)
			{
			}

			public override bool IsAssignableFrom(IType other)
			{
				var otherExternalType = other as ExternalType;
				return otherExternalType == null || otherExternalType.ActualType != Types.Void;
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

			public override bool IsVoid
			{
				get { return true; }
			}
		}

		#endregion

	}
}
