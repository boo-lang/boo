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

using Boo.Lang.Compiler.TypeSystem.Internal;

namespace Boo.Lang.Compiler.TypeSystem
{
	using System;
	using Boo.Lang.Compiler.Ast;

	public class InternalEnumMember : InternalEntity<EnumMember>, IField
	{	
		public InternalEnumMember(EnumMember member) : base(member)
		{
		}
		
		override public bool IsStatic
		{
			get { return true; }
		}
		
		override public bool IsPublic
		{
			get { return true; }
		}
		
		override public bool IsProtected
		{
			get { return false; }
		}
		
		public bool IsLiteral
		{
			get { return true; }
		}

		override public bool IsInternal
		{
			get { return false; }
		}

		override public bool IsPrivate
		{
			get { return false; }
		}
		
		public bool IsInitOnly
		{
			get { return false;  }
		}
		
		override public EntityType EntityType
		{
			get { return EntityType.Field; }
		}
		
		public IType Type
		{
			get { return DeclaringType; }
		}
		
		public object StaticValue
		{
			get {
				if (_node.Initializer.NodeType == NodeType.IntegerLiteralExpression)
				{
					return Convert.ChangeType(((IntegerLiteralExpression) _node.Initializer).Value,
							((InternalEnum) DeclaringType).UnderlyingType);
				}
				return Error.Default;
			}
		}

		public bool IsVolatile
		{
			get { return false; }
		}

		public bool IsDuckTyped
		{
			get { return false; }
		}
	}
}
