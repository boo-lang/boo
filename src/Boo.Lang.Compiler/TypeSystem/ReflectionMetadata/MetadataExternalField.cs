#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
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
using System.Linq;

namespace Boo.Lang.Compiler.TypeSystem.ReflectionMetadata
{
	using System.Reflection.Metadata;
	using FieldAttributes = System.Reflection.FieldAttributes;

	public class MetadataExternalField: MetadataExternalEntity<FieldDefinition>, IField
	{
		private readonly MetadataExternalType _parent;
		private readonly IType _type;
		private object _value;

		public MetadataExternalField(
			MetadataTypeSystemProvider provider,
			FieldDefinition fd,
			MetadataExternalType parent,
			MetadataReader reader)
			: base(provider, fd, reader)
		{
			_parent = parent;
			_type = fd.DecodeSignature(new MetadataSignatureDecoder(provider, reader), new MetadataGenericContext(null, null));
			if (IsLiteral)
			{
				var constant = _reader.GetConstant(_memberInfo.GetDefaultValue());
				var blobReader = _reader.GetBlobReader(constant.Value);
				_value = blobReader.ReadConstant(constant.TypeCode);
			}
		}

		public virtual IType DeclaringType
		{
			get { return _parent; }
		}

		public bool IsPublic
		{
			get { return (_memberInfo.Attributes & FieldAttributes.Public) != 0; }
		}

		public bool IsProtected
		{
			get { return (_memberInfo.Attributes & (FieldAttributes.Family | FieldAttributes.FamORAssem)) != 0; }
		}

		public bool IsPrivate
		{
			get { return (_memberInfo.Attributes & FieldAttributes.Private) != 0; }
		}

		public bool IsInternal
		{
			get { return (_memberInfo.Attributes & FieldAttributes.Assembly) == FieldAttributes.Assembly; }
		}

		public bool IsStatic
		{
			get { return (_memberInfo.Attributes & FieldAttributes.Static) != 0; }
		}

		public bool IsLiteral
		{
			get { return (_memberInfo.Attributes & FieldAttributes.Literal) != 0; }
		}

		public bool IsInitOnly
		{
			get { return (_memberInfo.Attributes & FieldAttributes.InitOnly) != 0; }
		}

		override public EntityType EntityType
		{
			get { return EntityType.Field; }
		}

		public virtual IType Type
		{
			get { return _type; }
		}

		public object StaticValue
		{
			get { return _value; }
		}

		static readonly Type IsVolatileType = typeof(System.Runtime.CompilerServices.IsVolatile);
		bool? _isVolatile;

		public bool IsVolatile
		{
			get
			{
				if (_isVolatile == null)
				{
					var mod = _type as IModifiedType;
					if (mod != null)
					{
						var volType = _provider.Map(IsVolatileType);
						_isVolatile = mod.ModReqs.Contains(volType);
					}
					else _isVolatile = false;
				}
				return _isVolatile.Value;
			}
		}

		public FieldDefinition FieldInfo
		{
			get { return _memberInfo; }
		}

		protected override IType MemberType
		{
			get { return _type; }
		}

		public override string Name
		{
			get { return _reader.GetString(_memberInfo.Name); }
		}

		protected override string BuildFullName()
		{
			return _parent.FullName + "." + Name;
		}

		protected override bool HasAttribute(IType attr)
		{
			var coll = _memberInfo.GetCustomAttributes();
			if (coll.Count == 0)
				return false;
			var attrs = _provider.GetCustomAttributeTypes(coll, _reader);
			return attrs.Contains(attr);
		}
	}
}
