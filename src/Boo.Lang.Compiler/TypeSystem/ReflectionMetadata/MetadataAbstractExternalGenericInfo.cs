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

using Boo.Lang.Compiler.TypeSystem.Core;
using Boo.Lang.Compiler.Util;

namespace Boo.Lang.Compiler.TypeSystem.ReflectionMetadata
{
	using System;
	using System.Collections.Generic;
	using System.Reflection.Metadata;

	public abstract class MetadataAbstractExternalGenericInfo<T> where T : IEntity
	{
		protected readonly MetadataTypeSystemProvider _provider;
		private IGenericParameter[] _parameters;
		private Dictionary<IType[], T> _instances = new Dictionary<IType[], T>(ArrayEqualityComparer<IType>.Default);
		protected readonly MetadataReader _reader;
		protected readonly MetadataExternalType _parent;
		protected readonly MetadataExternalMethod _method;

		protected MetadataAbstractExternalGenericInfo(
			MetadataTypeSystemProvider provider,
			MetadataExternalType parent,
			MetadataReader reader)
		{
			_provider = provider;
			_reader = reader;
			_parent = parent;
		}

		protected MetadataAbstractExternalGenericInfo(
			MetadataTypeSystemProvider provider,
			MetadataExternalMethod parent,
			MetadataReader reader)
		{
			_provider = provider;
			_reader = reader;
			_method = parent;
			_parent = (MetadataExternalType)parent.DeclaringType;
		}

		public IGenericParameter[] GenericParameters
		{
			get
			{
				if (null == _parameters)
				{
					if (_method != null)
						_parameters = Array.ConvertAll(
							GetActualGenericParameters(),
							t => new MetadataExternalGenericParameter(_provider, t, _method, _reader));
					else
						_parameters = Array.ConvertAll(
							GetActualGenericParameters(),
							t => new MetadataExternalGenericParameter(_provider, t, _parent, _reader));
				}
				return _parameters;
			}
		}

		protected T ConstructEntity(IType[] arguments)
		{
			if (_instances.ContainsKey(arguments))
			{
				return _instances[arguments];
			}

			T instance = ConstructInternalEntity(arguments);
			_instances.Add(arguments, instance);
			return instance;
		}

		protected abstract GenericParameter[] GetActualGenericParameters();
		protected abstract T ConstructInternalEntity(IType[] arguments);

		private bool IsExternal(IType type)
		{
			if (type is MetadataExternalType)
			{
				return true;
			}

			if (type is ArrayType)
			{
				return IsExternal(type.ElementType);
			}

			return false;
		}
	}
}