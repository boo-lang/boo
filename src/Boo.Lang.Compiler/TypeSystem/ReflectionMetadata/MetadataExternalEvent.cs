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

using System.Linq;
using System.Reflection.Metadata;

namespace Boo.Lang.Compiler.TypeSystem.ReflectionMetadata
{
	public class MetadataExternalEvent : MetadataExternalEntity<EventDefinition>, IEvent
	{
		private readonly MetadataExternalType _parent;
		private readonly IMethod _add;
		private readonly IMethod _fire;
		private readonly IMethod _remove;
		private readonly IType _type;

		public MetadataExternalEvent(
			MetadataTypeSystemProvider provider,
			EventDefinition ed,
			MetadataExternalType parent,
			MetadataReader reader)
			: base(provider, ed, reader)
		{
			_parent = parent;
			var accessors = ed.GetAccessors();
			if (!accessors.Adder.IsNil)
				_add = provider.Map(parent, reader.GetMethodDefinition(accessors.Adder));
			if (!accessors.Raiser.IsNil)
				_fire = provider.Map(parent, reader.GetMethodDefinition(accessors.Raiser));
			if (!accessors.Remover.IsNil)
				_remove = provider.Map(parent, reader.GetMethodDefinition(accessors.Remover));
			_type = provider.GetTypeFromEntityHandle(ed.Type, reader);
		}

		public virtual IType DeclaringType
		{
			get { return _parent; }
		}

		public virtual IMethod GetAddMethod()
		{
			return _add;
		}

		public virtual IMethod GetRemoveMethod()
		{
			return _remove;
		}

		public virtual IMethod GetRaiseMethod()
		{
			return _fire;
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

		public EventDefinition EventInfo
		{
			get { return _memberInfo; }
		}

		public bool IsPublic
		{
			get { return GetAddMethod().IsPublic; }
		}

		override public EntityType EntityType
		{
			get { return EntityType.Event; }
		}

		public virtual IType Type
		{
			get
			{
				return _type;
			}
		}

		public bool IsStatic
		{
			get
			{
				return GetAddMethod().IsStatic;
			}
		}

		public bool IsAbstract
		{
			get
			{
				return GetAddMethod().IsAbstract;

			}
		}

		public bool IsVirtual
		{
			get
			{
				return GetAddMethod().IsVirtual;
			}
		}

		override protected IType MemberType
		{
			get { return _type; }
		}

		public override string Name
		{
			get { return _reader.GetString(_memberInfo.Name); }
		}
}
}