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

namespace Boo.Lang.Compiler.TypeSystem.Generics
{
	public abstract class TypeMapper
	{
		protected IDictionary<IEntity, IEntity> _cache = new Dictionary<IEntity, IEntity>();

		protected TypeSystemServices TypeSystemServices
		{
			get { return My<TypeSystemServices>.Instance; } 
		}

		protected TEntity Cache<TEntity>(TEntity source, TEntity mapped) where TEntity : IEntity
		{
			_cache[source] = mapped;
			return mapped;
		}

		public virtual IType MapType(IType sourceType)
		{
			if (sourceType == null) return null;

			if (_cache.ContainsKey(sourceType)) return (IType)_cache[sourceType];

			if (sourceType.IsByRef)
			{
				return MapByRefType(sourceType);
			}

			if (sourceType.ConstructedInfo != null)
			{
				return MapConstructedType(sourceType);
			}

			// TODO: Map nested types
			// GenericType[of T].NestedType => GenericType[of int].NestedType

			IArrayType array = (sourceType as IArrayType);
			if (array != null) return MapArrayType(array);

			AnonymousCallableType anonymousCallableType = sourceType as AnonymousCallableType;
			if (anonymousCallableType != null)
			{
				return MapCallableType(anonymousCallableType);
			}

			return sourceType;
		}

		public virtual IType MapByRefType(IType sourceType)
		{
			IType elementType = MapType(sourceType.GetElementType());
			return elementType;
		}

		public virtual IType MapArrayType(IArrayType sourceType)
		{
			IType elementType = MapType(sourceType.GetElementType());
			return elementType.MakeArrayType(sourceType.GetArrayRank());
		}

		public virtual IType MapCallableType(AnonymousCallableType sourceType)
		{
			CallableSignature signature = sourceType.GetSignature();

			IType returnType = MapType(signature.ReturnType);
			IParameter[] parameters = MapParameters(signature.Parameters);

			CallableSignature mappedSignature = new CallableSignature(
				parameters, returnType, signature.AcceptVarArgs);

			return TypeSystemServices.GetCallableType(mappedSignature);
		}

		public virtual IType MapConstructedType(IType sourceType)
		{
			IType mappedDefinition = MapType(sourceType.ConstructedInfo.GenericDefinition);

			IType[] mappedArguments = Array.ConvertAll<IType, IType>(
				sourceType.ConstructedInfo.GenericArguments,
				MapType);

			IType mapped = mappedDefinition.GenericInfo.ConstructType(mappedArguments);

			return mapped;
		}

		internal IParameter[] MapParameters(IParameter[] parameters)
		{
			return Array.ConvertAll<IParameter, IParameter>(parameters, MapParameter);
		}

		internal IParameter MapParameter(IParameter parameter)
		{
			if (_cache.ContainsKey(parameter)) return _cache[parameter] as IParameter;
			return Cache<IParameter>(parameter, new MappedParameter(parameter, MapType(parameter.Type)));
		}
	}

	public class TypeReplacer : TypeMapper
	{
		IDictionary<IType, IType> _map;

		public TypeReplacer()
		{
			_map = new Dictionary<IType, IType>();
		}
		
		protected IDictionary<IType, IType> TypeMap
		{
			get { return _map; }
		}

		public void Replace(IType source, IType replacement)
		{
			_map[source] = replacement;
		}

		public override IType MapType(IType sourceType)
		{
			if (TypeMap.ContainsKey(sourceType)) return TypeMap[sourceType];
			
			return base.MapType(sourceType);
		}
	}
}