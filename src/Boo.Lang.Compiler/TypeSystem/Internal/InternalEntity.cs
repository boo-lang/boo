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
using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.TypeSystem
{
	public abstract class InternalEntity<T> : IInternalEntity, IEntityWithAttributes where T : TypeMember
	{
		protected readonly T _node;

		public InternalEntity(T node)
		{
			_node = node;
		}

		#region IInternalEntity Members
		public Node Node
		{
			get { return _node; }
		}
		#endregion

		#region IEntity Members
		public string Name
		{
			get { return _node.Name; }
		}

		public virtual string FullName
		{
			get { return _node.DeclaringType.FullName + "." + _node.Name; }
		}
		#endregion

		public IType DeclaringType
		{
			get { return (IType)TypeSystemServices.GetEntity(_node.DeclaringType); }
		}

		public bool IsDefined(IType type)
		{
			return MetadataUtil.IsAttributeDefined(_node, type);
		}

		public virtual bool IsStatic
		{
			get { return _node.IsStatic; }
		}

		public virtual bool IsPublic
		{
			get { return _node.IsPublic; }
		}

		public virtual bool IsProtected
		{
			get { return _node.IsProtected; }
		}

		public virtual bool IsPrivate
		{
			get { return _node.IsPrivate; }
		}

		public virtual bool IsInternal
		{
			get { return _node.IsInternal; }
		}

		public abstract EntityType EntityType { get; }
	}
}
