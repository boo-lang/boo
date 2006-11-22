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
	using System.Collections.Generic;

	public class ExternalGenericTypeDefinitionInfo : IGenericTypeDefinitionInfo
	{
		private ExternalType _type;
		private TypeSystemServices _tss;
		private IGenericParameter[] _parameters;
		private IDictionary<IType[], IType> _instances;

		public ExternalGenericTypeDefinitionInfo(TypeSystemServices tss, ExternalType type)
		{	
			_tss = tss;
			_type = type;
			_instances = new Dictionary<IType[], IType>(new GenericArgumentsComparer());
		}

		public IGenericParameter[] GenericParameters
		{
			get
			{
				if (null == _parameters)
				{
					_parameters = Array.ConvertAll<Type, ExternalGenericParameter>(
						_type.ActualType.GetGenericArguments(),
						delegate(Type t) { return new ExternalGenericParameter(_tss, t); });
				}
				return _parameters;
			}
		}

		public IType MakeGenericType(IType[] arguments)
		{
			if (Array.TrueForAll(arguments, IsExternal))
			{
				Type[] actualTypes = Array.ConvertAll<IType, Type>(arguments, GetSystemType);

				return _tss.Map(_type.ActualType.MakeGenericType(actualTypes));
			}
			else if (_instances.ContainsKey(arguments))
			{
				return _instances[arguments];
			}
			else
			{
				IType instance = CreateMixedType(arguments);
				_instances.Add(arguments, instance);
				
				return instance;
			}
		}
		
		private IType CreateMixedType(IType[] arguments)
		{
			ExternalCallableType callable = _type as ExternalCallableType;
			if (null != callable)
			{
				return new MixedGenericCallableType(_tss, callable, arguments);
			}
			else
			{
				return new MixedGenericType(_tss, _type, arguments);
			}
		}
				
		private bool IsExternal(IType type)
		{
			if (type is ExternalType && !(type is MixedGenericType))
			{
				return true;				
			}
			
			if (type is ArrayType)
			{
				return IsExternal(type.GetElementType());
			}
			
			return false;
		}
		
		private Type GetSystemType(IType type)
		{
			if (type is ExternalType) 
			{
				return ((ExternalType)type).ActualType;
			}
			
			ArrayType arrayType = type as ArrayType;
			if (arrayType != null)
			{			
				Type elementType = GetSystemType(arrayType.GetElementType());
				
				return Array.CreateInstance(
					elementType, 
					new int[arrayType.GetArrayRank()]).GetType();
			}
			
			return null;
		}
		
		private class GenericArgumentsComparer: IEqualityComparer<IType[]>
		{
			public bool Equals(IType[] x, IType[] y)
			{
				for (int i = 0; i < x.Length; i++)
				{
					if ((x[i] == null && y[i] != null) || (!x[i].Equals(y[i])))
					{
						return false;
					}
				}
				
				return true;
			}
			
			public int GetHashCode(IType[] args)
			{
				// Make a simple hash code from the hash codes of the arguments
				int hash = 0;
				for (int i = 0; i < args.Length; i++)
				{
					hash ^= i ^ args[i].GetHashCode();
				}
				
				return hash;
			}
		}
	}
	
			
	public class ExternalGenericParameter : ExternalType, IGenericParameter
	{
		public ExternalGenericParameter(TypeSystemServices tss, Type type) : base(tss, type)
		{
		}
		
		public int GenericParameterPosition
		{
			get { return ActualType.GenericParameterPosition; }
		}
		
		public override string FullName 
		{
			get 
			{
				return string.Format("{0}.{1}", DeclaringType.FullName, Name);
			}
		}
	}
}
#endif

