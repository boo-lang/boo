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
using Boo.Lang.Compiler.Util;
using Microsoft.Cci;

namespace Boo.Lang.Compiler.TypeSystem.Cci
{
    public class AssemblyReferenceCci : IAssemblyReferenceCci, IEquatable<AssemblyReferenceCci>
	{
		private readonly IAssembly _assembly;
		private readonly CciTypeSystemProvider _provider;
		private INamespace _rootNamespace;
	    private readonly HashSet<INamedTypeDefinition> _types;
        private readonly ITypeReference _multicastType;

        private readonly MemoizedFunction<ITypeReference, IType> _typeEntityCache;
		private readonly MemoizedFunction<ITypeMemberReference, IEntity> _memberCache;

		internal AssemblyReferenceCci(CciTypeSystemProvider provider, IAssembly assembly)
		{
			if (null == assembly)
				throw new ArgumentNullException("assembly");
			_provider = provider;
			_assembly = assembly;
            _typeEntityCache = new MemoizedFunction<ITypeReference, IType>(NewType);
            _memberCache = new MemoizedFunction<ITypeMemberReference, IEntity>(NewEntityForMember);
		    _types = new HashSet<INamedTypeDefinition>(assembly.GetAllTypes());
		    _multicastType = SystemTypeMapper.GetTypeReference(Types.MulticastDelegate);
		}
		
		public string Name
		{
            get { return _name ?? (_name = FullName); }
		}

		string _name;

		public string FullName
		{
			get { return _assembly.Name.Value; }
		}
		
		public EntityType EntityType
		{
			get { return EntityType.Assembly; }
		}
		
		public IAssembly Assembly
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
			return new CciNamespaceBuilder(_provider, _assembly).Build();
		}

		#endregion

		public override bool Equals(object other)
		{
			if (null == other) return false;
			if (this == other) return true;

            AssemblyReferenceCci aref = other as AssemblyReferenceCci;
			return Equals(aref);
		}

        public bool Equals(AssemblyReferenceCci other)
		{
			if (null == other) return false;
			if (this == other) return true;

			return IsReferencing(other._assembly);
		}

		private bool IsReferencing(IAssembly assembly)
		{
			return AssemblyEqualityComparer.Default.Equals(_assembly, assembly);
		}

		public override int GetHashCode()
		{
			return AssemblyEqualityComparer.Default.GetHashCode(_assembly);
		}

		public override string ToString()
		{
			return FullName;
		}

		public IType Map(ITypeReference type)
		{
			AssertAssembly(type);
			return _typeEntityCache.Invoke(type);
		}

        public IEntity MapMember(ITypeMemberReference mi)
		{
			AssertAssembly(mi);
		    var type = mi as INestedTypeReference;
			if (type != null)
				return Map(type);
			return _memberCache.Invoke(mi);
		}

        private void AssertAssembly(ITypeReference member)
        {
            var rt = (INamedTypeDefinition)member.ResolvedType;
            if (_types.Contains(rt))
                throw new ArgumentException(string.Format("{0} doesn't belong to assembly '{1}'.", member, _assembly));
        }

        private void AssertAssembly(ITypeMemberReference member)
		{
            var rt = (INamedTypeDefinition)(member.ContainingType).ResolvedType;
            if (_types.Contains(rt))
                throw new ArgumentException(string.Format("{0} doesn't belong to assembly '{1}'.", member, _assembly));
		}

        private IEntity NewEntityForMember(ITypeMemberReference mi)
        {
            var method = mi as IMethodDefinition;
		    if (method != null)
		    {
		        return new ExternalMethod(_provider, method);
		    }
            var field = mi as IFieldDefinition;
		    if (field != null)
				return new ExternalField(_provider, field);
            var prop = mi as IPropertyDefinition;
            if (prop != null)
				return new ExternalProperty(_provider, prop);
            var ev = mi as IEventDefinition;
			if (ev != null)
				return new ExternalEvent(_provider, ev);
			throw new NotImplementedException(mi.ToString());
		}

        public void MapTo(INamedTypeDefinition type, IType entity)
		{
			AssertAssembly(type);
			_typeEntityCache.Add(type, entity);
		}

        private IType NewType(ITypeReference type)
        {
            var arr = type as IArrayTypeReference;
			return arr != null
				? Map(arr.ElementType).MakeArrayType((int)arr.Rank)
				: CreateEntityForType(type);
		}

        private IType CreateEntityForType(ITypeReference type)
        {
            var gp = type as Microsoft.Cci.IGenericParameter;
			if (gp != null) return new ExternalGenericParameter(_provider, gp);
            var rt = type.ResolvedType;
			if (TypeHelper.Type1DerivesFromOrIsTheSameAsType2(rt, _multicastType, true))
                return _provider.CreateEntityForCallableType((INamedTypeDefinition)rt);
            return _provider.CreateEntityForRegularType((INamedTypeDefinition)rt);
		}

	}
}
