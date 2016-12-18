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
using Boo.Lang.Compiler.TypeSystem.Generics;
using Microsoft.Cci;
using Boo.Lang.Compiler.TypeSystem.Cci;

namespace Boo.Lang.Compiler.TypeSystem
{
	public class ExternalConstructedTypeInfo : IConstructedTypeInfo
	{
		private readonly ExternalType _type;
        private readonly ICciTypeSystemProvider _tss;
        private IType[] _arguments;
		private GenericMapping _mapping;

        public ExternalConstructedTypeInfo(ICciTypeSystemProvider tss, ExternalType type)
		{
			_type = type;
			_tss = tss;
		}

		protected GenericMapping GenericMapping
		{
			get { return _mapping ?? (_mapping = new ExternalGenericMapping(_tss, _type, GenericArguments)); }
		}

		public IType GenericDefinition
		{
			get 
			{

                return _tss.Map(((IGenericTypeInstance)_type.ActualType).GenericType.ResolvedType);
			}
		}
		
		public IType[] GenericArguments
		{
			get
			{
			    return _arguments ?? (_arguments = _type.ActualType.GenericParameters
			               .Select(gp => _tss.Map(((ITypeReference) gp).ResolvedType))
			               .ToArray());
			}
		}
					
		public bool FullyConstructed	
		{
		    get
		    {
		        return !_type.ActualType.ContainsGenericParameters();
		    }
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