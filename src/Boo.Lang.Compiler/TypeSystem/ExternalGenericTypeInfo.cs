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

#if NET_2_0

namespace Boo.Lang.Compiler.TypeSystem
{
	using System;

	public class ExternalGenericTypeInfo : IGenericTypeInfo
	{
		TypeSystemServices _tss;
		ExternalType _type;
		IType[] _arguments = null;
		bool _constructed;
		
		public ExternalGenericTypeInfo(TypeSystemServices tss, ExternalType type)
		{
			_tss = tss;
			_type = type;
			_constructed = !_type.ActualType.ContainsGenericParameters;
		}		

		public IType[] GenericArguments
		{
			get 
			{
				if (_arguments == null)
				{
					_arguments = Array.ConvertAll<Type, IType>(
						_type.ActualType.GetGenericArguments(),
						_tss.Map);
				}
				
				return _arguments;
			}
		}
		
		public IType GenericDefinition
		{
			get 
			{
				return _tss.Map(_type.ActualType.GetGenericTypeDefinition());
			}
		}
		
		public bool FullyConstructed
		{
			get { return _constructed; }
		}		
	}	
}
#endif
