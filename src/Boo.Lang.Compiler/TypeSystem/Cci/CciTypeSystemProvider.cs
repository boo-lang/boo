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
using Boo.Lang.Compiler.Util;
using Microsoft.Cci;

namespace Boo.Lang.Compiler.TypeSystem.Cci
{
	public class CciTypeSystemProvider : ICciTypeSystemProvider
	{
        private readonly MemoizedFunction<IUnit, AssemblyReferenceCci> _referenceCache;
		
		public CciTypeSystemProvider()
		{
            _referenceCache = new MemoizedFunction<IUnit, AssemblyReferenceCci>(AssemblyEqualityComparer.Default, CreateReference);
			Initialize();
		}

        protected virtual void Initialize()
		{
			MapTo(typeof(object), new ObjectTypeImpl(this));
			MapTo(typeof(Builtins.duck), new ObjectTypeImpl(this));
			MapTo(typeof(void), new VoidTypeImpl(this));
		}

	    private void MapTo(Type type, IType entity)
	    {
	        MapTo((INamedTypeDefinition)SystemTypeMapper.GetTypeReference(type).ResolvedType, entity);
	    }

		protected void MapTo(INamedTypeDefinition type, IType entity)
		{
			AssemblyReferenceFor(type).MapTo(type, entity);
		}

        private CciTypeSystemProvider(MemoizedFunction<IUnit, AssemblyReferenceCci> referenceCache)
		{
			_referenceCache = referenceCache;
		}

		#region Implementation of ICompilerReferenceProvider

		public AssemblyReferenceCci ForAssembly(IUnit assembly)
		{
			return AssemblyReferenceFor(assembly);
		}

        private AssemblyReferenceCci AssemblyReferenceFor(INamedTypeDefinition type)
	    {
	        INestedTypeDefinition nest;
	        var currentType = type;
	        while ((nest = currentType as INestedTypeDefinition) != null)
	        {
	            currentType = (INamedTypeDefinition) nest.ContainingTypeDefinition;
	        }
	        var nsType = (INamespaceTypeDefinition) currentType;
	        return ForAssembly(nsType.ContainingUnitNamespace.Unit);
	    }

        private AssemblyReferenceCci AssemblyReferenceFor(IUnit assembly)
		{
			return _referenceCache.Invoke(assembly);
		}

        private AssemblyReferenceCci CreateReference(IUnit assembly)
		{
            return new AssemblyReferenceCci(this, (IAssembly)assembly);
		}

		#endregion

		#region Implementation of IReflectionTypeSystemProvider

        public IType Map(ITypeDefinition type)
		{
			return AssemblyReferenceFor((INamedTypeDefinition)type).Map(type);
		}

		public IMethod Map(IMethodDefinition method)
		{
			return (IMethod) MapMember(method);
		}

        public IEntity Map(ITypeDefinitionMember mi)
		{
			return MapMember(mi);
		}

        public IParameter[] Map(IParameterDefinition[] parameters)
		{
			var mapped = new IParameter[parameters.Length];
			for (int i = 0; i < parameters.Length; ++i)
			{
				mapped[i] = new ExternalParameter(this, parameters[i]);
			}
			return mapped;
		}

        private IEntity MapMember(ITypeDefinitionMember mi)
		{
			return AssemblyReferenceFor((INamedTypeDefinition)mi.ContainingTypeDefinition).MapMember(mi);
		}

        public virtual ICciTypeSystemProvider Clone()
		{
			return new CciTypeSystemProvider(_referenceCache.Clone());
		}

		public virtual IType CreateEntityForRegularType(Type type)
		{
			return new ExternalType(this, type);
		}

		public virtual IType CreateEntityForCallableType(Type type)
		{
			return new ExternalCallableType(this, type);
		}

        public IEntity Map(ITypeDefinitionMember[] members)
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

		private sealed class ObjectTypeImpl : ExternalType
		{
            internal ObjectTypeImpl(ICciTypeSystemProvider provider)
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
		private sealed class VoidTypeImpl : ExternalType
		{
            internal VoidTypeImpl(ICciTypeSystemProvider provider)
				: base(provider, Types.Void)
			{
			}

            public override bool Resolve(ICollection<IEntity> resultingSet, string name, EntityType typesToConsider)
			{
				return false;
			}

            public override bool IsSubclassOf(IType other)
			{
				return false;
			}

            public override bool IsAssignableFrom(IType other)
			{
				return false;
			}
		}

		#endregion

	}
}
