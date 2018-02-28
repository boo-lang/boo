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


using System;
using System.Reflection.Metadata;

namespace Boo.Lang.Compiler.TypeSystem.ReflectionMetadata
{
	public abstract class MetadataExternalEntity<T> : IEntity, IEntityWithAttributes
		where T : struct
	{
		protected readonly T _memberInfo;

		protected readonly MetadataReader _reader;

		private string _cachedFullName;

		protected readonly MetadataTypeSystemProvider _provider;

		private bool? _isDuckTyped;
		private bool? _isExtension;

		public MetadataExternalEntity(MetadataTypeSystemProvider typeSystemServices, T memberInfo, MetadataReader reader)
		{
			_provider = typeSystemServices;
			_memberInfo = memberInfo;
			_reader = reader;
		}

		public abstract string Name { get; }

		public string FullName
		{
			get
			{
				if (_cachedFullName != null) return _cachedFullName;
				return _cachedFullName = BuildFullName();
			}
		}

		protected abstract string BuildFullName();

		protected IType Map(TypeDefinition type)
		{
			return _provider.Map(type, _reader);
		}

		public abstract EntityType EntityType { get; }

		protected abstract IType MemberType { get; }

		public override string ToString()
		{
			return FullName;
		}

		protected abstract bool HasAttribute(IType attr);

		public bool IsDuckTyped
		{
			get
			{
				if (!_isDuckTyped.HasValue)
					_isDuckTyped =
						!MemberType.IsValueType && HasAttribute(_provider.Map(Types.DuckTypedAttribute));
				return _isDuckTyped.Value;
			}
		}

		public bool IsExtension
		{
			get
			{
				if (!_isExtension.HasValue)
					_isExtension = IsClrExtension;
				return _isExtension.Value;
			}
		}

		private bool IsClrExtension
		{
			get { return HasAttribute(_provider.Map(Types.ClrExtensionAttribute)); }
		}

		public bool IsDefined(IType attributeType)
		{
			return HasAttribute(attributeType);
		}
	}
}
