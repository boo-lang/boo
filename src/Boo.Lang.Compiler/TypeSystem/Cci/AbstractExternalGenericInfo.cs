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

using System.Linq;
using Boo.Lang.Compiler.TypeSystem.Cci;
using Boo.Lang.Compiler.TypeSystem.Core;
using Boo.Lang.Compiler.Util;
using Microsoft.Cci;

namespace Boo.Lang.Compiler.TypeSystem
{
	using System;
	using System.Collections.Generic;

	public abstract class AbstractExternalGenericInfo<T> where T : IEntity
	{
        protected ICciTypeSystemProvider _provider;
		private IGenericParameter[] _parameters;
		private readonly Dictionary<IType[], T> _instances = new Dictionary<IType[], T>(ArrayEqualityComparer<IType>.Default);

        protected AbstractExternalGenericInfo(ICciTypeSystemProvider provider)
		{	
			_provider = provider;
		}

		public IGenericParameter[] GenericParameters
		{
			get
			{
				if (null == _parameters)
				{
					_parameters = GetActualGenericParameters()
                        .Select(t => (ExternalGenericParameter)_provider.Map(((ITypeReference)t).ResolvedType))
                        .Cast<IGenericParameter>()
                        .ToArray();
				}
				return _parameters;
			}
		}

		protected T ConstructEntity(IType[] arguments)
		{
			if (Array.TrueForAll(arguments, IsExternal))
			{
				ITypeReference[] actualTypes = Array.ConvertAll(arguments, GetSystemType);

				return ConstructExternalEntity(actualTypes);
			}
			if (_instances.ContainsKey(arguments))
			{
				return _instances[arguments];
			}

			T instance = ConstructInternalEntity(arguments);
			_instances.Add(arguments, instance);
			return instance;
		}

        protected abstract Microsoft.Cci.IGenericParameter[] GetActualGenericParameters();
		protected abstract T ConstructInternalEntity(IType[] arguments);
        protected abstract T ConstructExternalEntity(IEnumerable<ITypeReference> arguments);
		
		private static bool IsExternal(IType type)
		{
			if (type is ExternalType)
			{
				return true;				
			}
			
			if (type is ArrayType)
			{
				return IsExternal(type.ElementType);
			}
			
			return false;
		}

        private ITypeReference GetSystemType(IType type)
		{
			// Get system type from external types
			ExternalType et = type as ExternalType;
			if (null != et)
				return et.ActualType;

			// Get system array types from arrays of external types
			ArrayType arrayType = type as ArrayType;
			if (arrayType != null)
			{			
				var elementType = GetSystemType(arrayType.ElementType);
				int rank = arrayType.Rank;

				// Calling MakeArrayType(1) gives a multi-dimensional array with 1 dimensions,
				// which is (surprisingly) not the same as calling MakeArrayType() which gives
				// a single-dimensional array
			    var factory = CompilerContext.Current.Host.InternFactory;
                return (rank == 1 
                    ? (ITypeReference)Microsoft.Cci.Immutable.Vector.GetVector(elementType, factory) 
                    : Microsoft.Cci.Immutable.Matrix.GetMatrix(elementType, (uint)rank, factory));
			}
			
			// This shouldn't happen since we only call GetSystemType on external types or arrays of such
			return null;
		}
	}		
}
