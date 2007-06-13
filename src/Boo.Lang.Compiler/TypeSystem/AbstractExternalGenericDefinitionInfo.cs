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
	using System.Collections.Generic;

	public abstract class AbstractExternalGenericDefinitionInfo
	{
		protected TypeSystemServices _tss;
		private IGenericParameter[] _parameters;
		private Dictionary<IType[], IEntity> _instances = 
			new Dictionary<IType[], IEntity>(new ArrayEqualityComparer<IType>());

		public AbstractExternalGenericDefinitionInfo(TypeSystemServices tss)
		{	
			_tss = tss;
		}

		public IGenericParameter[] GenericParameters
		{
			get
			{
				if (null == _parameters)
				{
					_parameters = Array.ConvertAll<Type, ExternalGenericParameter>(
						GetActualGenericParameters(),
						delegate(Type t) { return (ExternalGenericParameter)_tss.Map(t); });
				}
				return _parameters;
			}
		}

		protected IEntity MakeGenericEntity(IType[] arguments)
		{
			if (Array.TrueForAll(arguments, IsExternal))
			{
				Type[] actualTypes = Array.ConvertAll<IType, Type>(arguments, GetSystemType);

				return MakeExternalEntity(actualTypes);
			}
			else if (_instances.ContainsKey(arguments))
			{
				return _instances[arguments];
			}
			else
			{
				IEntity instance = MakeMixedEntity(arguments);
				_instances.Add(arguments, instance);
				
				return instance;
			}
		}
		
		protected abstract Type[] GetActualGenericParameters();
		protected abstract IEntity MakeMixedEntity(IType[] arguments);
		protected abstract IEntity MakeExternalEntity(Type[] arguments);
		
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
	}		
}
