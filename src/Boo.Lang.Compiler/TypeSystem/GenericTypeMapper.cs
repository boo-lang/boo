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

namespace Boo.Lang.Compiler.TypeSystem
{
	using System;
	using System.Text;
	using System.Reflection;
	using System.Collections.Generic;

	public interface ITypeMapper
	{
		IType MapType(IType sourceType);
	}

	/// <summary>
	/// A basic mapper of generic parameters into arguments.
	/// </summary>
	public class GenericTypeMapper : ITypeMapper
	{
		#region Data Members
		
		TypeSystemServices _tss;
		Dictionary<IGenericParameter, IType> _map = new Dictionary<IGenericParameter, IType>();
		
		#endregion
		
		#region Constructor

		public GenericTypeMapper(TypeSystemServices tss, IGenericParameter[] parameters, IType[] arguments)
		{
			_tss = tss;
			for (int i = 0; i < parameters.Length; i++)
			{
				_map.Add(parameters[i], arguments[i]);
			}
		}
		
		/// <summary>
		/// Maps a type involving generic parameters to the corresponding type after substituting concrete
		/// arguments for generic parameters.
		/// </summary>
		/// <remarks>
		/// If the source type is a generic parameter, it is mapped to the corresponding argument.
		/// If the source type is an open generic type using any of the specified generic parameters, it 
		/// is mapped to a closed constructed type based on the specified arguments.
		/// TODO: complete this
		/// </remarks>
		public IType MapType(IType sourceType)
		{
			if (sourceType == null)
			{
				return null;
			}
			
			// If sourceType is a reference type, map its element type 
			if (sourceType.IsByRef)
			{
				return MapType(sourceType.GetElementType());
			}

			// Map generic parameter to corresponding argument
			IGenericParameter gp = sourceType as IGenericParameter;
			if (null != gp && _map.ContainsKey(gp))
			{
				return _map[gp];
			}

			// Map open constructed type using generic parameters to closed constructed type
			// using corresponding arguments
			if (null != sourceType.GenericTypeInfo)
			{
				IType[] mappedArguments = Array.ConvertAll<IType, IType>(
					sourceType.GenericTypeInfo.GenericArguments,
					MapType);
				
				IType mapped = sourceType.GenericTypeInfo.
					GenericDefinition.GenericTypeDefinitionInfo.
					MakeGenericType(mappedArguments);
				
				return mapped;
			}
			
			// Map array types
			IArrayType array = (sourceType as IArrayType);
			if (array != null)
			{
				IType elementType = array.GetElementType();
				IType mappedElementType = MapType(elementType);
				if (mappedElementType != elementType)
				{
					return _tss.GetArrayType(mappedElementType, array.GetArrayRank());
				}
			}
			
			// Map callable types
			ICallableType callable = sourceType as ICallableType;
			if (callable != null)
			{
				CallableSignature signature = callable.GetSignature();

				IType returnType = MapType(signature.ReturnType);
				IParameter[] parameters = Array.ConvertAll<IParameter, IParameter>(
					signature.Parameters,
					delegate(IParameter p) { return new MappedParameter(_tss, (ExternalParameter)p, this); });
					
				CallableSignature mappedSignature = new CallableSignature(
					parameters, returnType, signature.AcceptVarArgs);
					
				return _tss.GetCallableType(mappedSignature);
			}
			
			// If source type doesn't require mapping, return it as is
			return sourceType;
		}

		#endregion
	}
}
