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

using Boo.Lang.Compiler.TypeSystem.Generics;
using Boo.Lang.Compiler.TypeSystem.Reflection;

namespace Boo.Lang.Compiler.TypeSystem
{
	using System;

	public class ExternalConstructedTypeInfo : IConstructedTypeInfo
	{
		ExternalType _type;
		IReflectionTypeSystemProvider _tss;
		IType[] _arguments = null;
		GenericMapping _mapping = null;
		
		public ExternalConstructedTypeInfo(IReflectionTypeSystemProvider tss, ExternalType type)
		{
			_type = type;
			_tss = tss;
		}

		protected GenericMapping GenericMapping
		{
			get
			{
				if (_mapping == null) _mapping = new ExternalGenericMapping(_type, GenericArguments);
				return _mapping;
			}
		}

		public IType GenericDefinition
		{
			get 
			{
				return _tss.Map(_type.ActualType.GetGenericTypeDefinition());
			}
		}
		
		public IType[] GenericArguments
		{
			get 
			{
				if (_arguments == null)
				{
					_arguments = Array.ConvertAll<Type, IType>(
						_type.ActualType.GetGenericArguments(), _tss.Map);
				}
				
				return _arguments;
			}
		}
					
		public bool FullyConstructed	
		{
			get { return !_type.ActualType.ContainsGenericParameters; }
		}

		public IMember UnMap(IMember mapped)
		{
			return GenericMapping.UnMap(mapped);
		}

		public IType Map(IType type)
		{
			if (type == GenericDefinition) return _type;
			return GenericMapping.MapType(type);
		}

		public IMember Map(IMember member)
		{
			return (IMember)GenericMapping.Map(member);
		}
	}	
}