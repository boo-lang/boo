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

	public class ExternalGenericType : ExternalType, IGenericType
	{
		IType[] _arguments = null;
		bool _constructed;
		
#if NET_2_0
		public ExternalGenericType(TypeSystemServices tss, Type type) : base(tss, type)
		{
			_constructed = !type.ContainsGenericParameters;
		}		

		public IType[] GetGenericArguments()
		{
			if (_arguments == null)
			{
				Type[] actualArguments = ActualType.GetGenericArguments();
				_arguments = new IType[actualArguments.Length];
				
				Converter<Type, IType> map = _typeSystemServices.Map;
				_arguments = Array.ConvertAll(actualArguments, map);
			}
			
			return _arguments;
		}
		
		public IGenericTypeDefinition GetGenericTypeDefinition()
		{
			return (IGenericTypeDefinition)_typeSystemServices.Map(ActualType.GetGenericTypeDefinition());
		}
		
		public bool FullyConstructed
		{
			get { return _constructed; }
		}		

#else
		public ExternalGenericType(TypeSystemServices tss, Type type) : base(tss, type)
		{
		}		

		IType[] IGenericType.GetGenericArguments()
		{
			throw new NotImplementedException();
		}
		
		IGenericTypeDefinition IGenericType.GetGenericTypeDefinition()
		{
			throw new NotImplementedException();
		}
		
		bool IGenericType.FullyConstructed
		{
			get { throw new NotImplementedException(); }
		}
#endif
	}
}
