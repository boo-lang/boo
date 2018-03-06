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

using Boo.Lang.Environments;

namespace Boo.Lang.Compiler.TypeSystem.ReflectionMetadata
{
	using System;
	using System.Linq;
	using System.Reflection.Metadata;
	using MethodAttributes = System.Reflection.MethodAttributes;

	public class MetadataExternalMethod : MetadataExternalEntity<MethodDefinition>, IMethod, IEquatable<MetadataExternalMethod>
	{
		protected IParameter[] _parameters;
		private bool _acceptVarArgs;
		private bool? _isPInvoke;
		private bool? _isMeta;
		private readonly MetadataExternalType _parent;
		private readonly MethodSignature<IType> _signature;

		internal MetadataExternalMethod(MetadataTypeSystemProvider provider, MethodDefinition md, MetadataExternalType parent, MetadataReader reader)
			: base(provider, md, reader)
		{
			_parent = parent;
			var decoder = new MetadataSignatureDecoder(provider, reader);
			_signature = md.DecodeSignature(decoder, GetGenericContext(parent, md));
			_acceptVarArgs = _signature.RequiredParameterCount != _signature.ParameterTypes.Length;
		}

		private MetadataGenericContext GetGenericContext(MetadataExternalType parent, MethodDefinition md)
		{
			var gi = parent.GenericInfo;
			var parentGenParams = gi != null ? gi.GenericParameters : Enumerable.Empty<IGenericParameter>();
			var gi2 = this.GenericInfo;
			var selfGenParams = gi2 != null ? gi2.GenericParameters : Enumerable.Empty<IGenericParameter>();
			return new MetadataGenericContext(parentGenParams, selfGenParams);
		}

		public bool IsMeta
		{
			get
			{
				if (_isMeta == null)
					_isMeta = IsStatic && this.HasAttribute(_provider.Map(typeof(Boo.Lang.MetaAttribute)));
				return _isMeta.Value;
			}
		}

		public bool IsPInvoke
		{
			get
			{
				if (_isPInvoke == null)
					_isPInvoke = IsStatic && this.HasAttribute(_provider.Map(Types.DllImportAttribute));
				return _isPInvoke.Value;
			}
		}

		public virtual IType DeclaringType
		{
			get { return _parent; }
		}

		public bool IsStatic
		{
			get { return (_memberInfo.Attributes & MethodAttributes.Static) != 0; }
		}

		public bool IsPublic
		{
			get { return (_memberInfo.Attributes & MethodAttributes.Public) != 0; }
		}

		public bool IsProtected
		{
			get { return (_memberInfo.Attributes & (MethodAttributes.Family | MethodAttributes.FamANDAssem)) != 0; }
		}

		public bool IsPrivate
		{
			get { return (_memberInfo.Attributes & MethodAttributes.Private) != 0; }
		}

		public bool IsAbstract
		{
			get { return (_memberInfo.Attributes & MethodAttributes.Abstract) != 0; }
		}

		public bool IsInternal
		{
			get { return (_memberInfo.Attributes & MethodAttributes.Assembly) != 0; }
		}

		public bool IsVirtual
		{
			get { return (_memberInfo.Attributes & MethodAttributes.Virtual) != 0; }
		}

		public bool IsFinal
		{
			get { return (_memberInfo.Attributes & MethodAttributes.Final) != 0; }
		}

		public bool IsSpecialName
		{
			get { return (_memberInfo.Attributes & MethodAttributes.SpecialName) != 0; }
		}

		public bool AcceptVarArgs
		{
			get
			{
				return _acceptVarArgs;
			}
		}

		private bool ParamHasCustomAttribute(Parameter parameter, Type attrType)
		{
			var attrs = parameter.GetCustomAttributes()
				.Select(h => _provider.GetTypeFromEntityHandle(_reader.GetCustomAttribute(h).Constructor, _reader));
			return attrs.Any(a => a.FullName.Equals(attrType.FullName));
		}

		override protected bool HasAttribute(IType attr)
		{
			var coll = _memberInfo.GetCustomAttributes();
			if (coll.Count == 0)
				return false;
			var attrs = _provider.GetCustomAttributeTypes(coll, _reader);
			return attrs.Contains(attr);
		}

		override public EntityType EntityType
		{
			get { return EntityType.Method; }
		}

		public ICallableType CallableType
		{
			get { return My<TypeSystemServices>.Instance.GetCallableType(this); }
		}

		public IType Type
		{
			get { return CallableType; }
		}

		public virtual IParameter[] GetParameters()
		{
			if (null != _parameters) return _parameters;
			return _parameters = _memberInfo.GetParameters()
				.Select((p, i) => new MetadataExternalParameter(_reader.GetParameter(p), _signature.ParameterTypes[i], _reader))
				.ToArray();
		}

		public virtual IType ReturnType
		{
			get
			{
				return _signature.ReturnType;
			}
		}

		public MethodDefinition MethodInfo
		{
			get { return _memberInfo; }
		}

		override public bool Equals(object other)
		{
			if (null == other) return false;
			if (this == other) return true;

			MetadataExternalMethod method = other as MetadataExternalMethod;
			return Equals(method);
		}

		public bool Equals(MetadataExternalMethod other)
		{
			if (null == other) return false;
			if (this == other) return true;

			return _memberInfo.Equals(other._memberInfo);
		}

		override public int GetHashCode()
		{
			return _memberInfo.GetHashCode();
		}

		override public string ToString()
		{
			return _memberInfo.ToString();
		}

		protected override string BuildFullName()
		{
			return _parent.FullName + "." + Name;
		}

		MetadataExternalGenericMethodInfo _genericMethodDefinitionInfo;
		public IGenericMethodInfo GenericInfo
		{
			get
			{
				if (MethodInfo.GetGenericParameters().Count == 0)
					return null;

				return _genericMethodDefinitionInfo ??
					   (_genericMethodDefinitionInfo = new MetadataExternalGenericMethodInfo(_provider, this, _parent, _reader));
			}
		}

		public virtual IConstructedMethodInfo ConstructedInfo
		{
			get
			{
				return null;
			}
		}

		protected override IType MemberType
		{
			get
			{
				return _signature.ReturnType;
			}
		}

		public override string Name
		{
			get { return _reader.GetString(_memberInfo.Name); }
		}
	}
}
