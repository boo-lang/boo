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

#if NET_2_0
	public class ExternalGenericTypeDefinition : ExternalType, IGenericTypeDefinition
	{
		private IGenericParameter[] _parameters;

		public ExternalGenericTypeDefinition(TypeSystemServices tss, Type type) : base(tss, type)
		{	
		}

		public IGenericParameter[] GetGenericParameters()
		{
			if (null == _parameters) _parameters = CreateParameters();
			return _parameters;
		}

		public IType MakeGenericType(IType[] arguments)
		{
			return _typeSystemServices.Map(ActualType.MakeGenericType(ToSystemType(arguments)));
		}

		private Type[] ToSystemType(IType[] arguments)
		{
			Type[] externalTypes = new Type[arguments.Length];
			for (int i = 0; i < arguments.Length; ++i)
			{
				ExternalType externalType = arguments[i] as ExternalType;
				if (null == externalType) throw new NotImplementedException("only generics for externally defined types for now");
				externalTypes[i] = externalType.ActualType;
			}
			return externalTypes;
		}

		protected override string BuildFullName()
		{
			string name = ActualType.FullName;
			return name.Substring(0, name.IndexOf('`'));
		}

		private IGenericParameter[] CreateParameters()
		{
			Type[] arguments = this.ActualType.GetGenericArguments();
			IGenericParameter[] parameters = new IGenericParameter[arguments.Length];
			for (int i=0; i<arguments.Length; ++i)
			{
				parameters[i] = new ExternalGenericParameter(_typeSystemServices, arguments[i]);
			}
			return parameters;
		}

		public class ExternalGenericParameter : ExternalType, IGenericParameter
		{
			bool _constructed;
			
			public ExternalGenericParameter(TypeSystemServices tss, Type type) : base(tss, type)
			{					
				_constructed = type.IsGenericParameter;
			}
			
			public IGenericTypeDefinition GetDeclaringType()
			{
				return (IGenericTypeDefinition)_typeSystemServices.Map(ActualType.DeclaringType);
			}
			
			public bool Constructed
			{
				get { return _constructed; }
			}
		}
	}
#else
	public class ExternalGenericTypeDefinition : ExternalType, IGenericTypeDefinition
	{
		public ExternalGenericTypeDefinition(TypeSystemServices tss, Type type) : base(tss, type)
		{
		}

		public IGenericParameter[] GetGenericParameters()
		{
			throw new NotImplementedException();
		}

		public IType MakeGenericType(IType[] arguments)
		{
			throw new NotImplementedException();
		}
	}
#endif
}
