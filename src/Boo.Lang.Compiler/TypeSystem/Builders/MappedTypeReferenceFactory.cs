#region license
// Copyright (c) 2009 Rodrigo B. de Oliveira (rbo@acm.org)
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


using System.Collections.Generic;
using System.Linq;
using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.TypeSystem.Builders
{
	public class MappedTypeReferenceFactory : ITypeReferenceFactory
	{
		private readonly ITypeReferenceFactory _typeReferenceFactory;
		private readonly IDictionary<IType, IType> _typeMappings;

		public MappedTypeReferenceFactory(ITypeReferenceFactory typeReferenceFactory, IDictionary<IType, IType> typeMappings)
		{
			_typeReferenceFactory = typeReferenceFactory;
			_typeMappings = typeMappings;
		}

		public TypeReference TypeReferenceFor(IType type)
		{
			return _typeReferenceFactory.TypeReferenceFor(Map(type));
		}

		private IType Map(IType type)
		{
			IType mappedType;
			if (_typeMappings.TryGetValue(type, out mappedType))
				return mappedType;

			if (type.IsArray)
			{
				var arrayType = ((IArrayType)type);
				var mappedElementType = Map(arrayType.ElementType);
				return mappedElementType.MakeArrayType(arrayType.Rank);
			}

			var constructedTypeInfo = type.ConstructedInfo;
			if (constructedTypeInfo != null)
				return
					constructedTypeInfo.GenericDefinition.GenericInfo.ConstructType(
						constructedTypeInfo.GenericArguments.Select(a => Map(a)).ToArray());
			return type;
		}
	}
}