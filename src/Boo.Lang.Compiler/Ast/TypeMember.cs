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

namespace Boo.Lang.Compiler.Ast
{
	[System.Xml.Serialization.XmlInclude(typeof(TypeDefinition))]
	[System.Xml.Serialization.XmlInclude(typeof(EnumMember))]
	[System.Xml.Serialization.XmlInclude(typeof(Field))]
	[System.Xml.Serialization.XmlInclude(typeof(Property))]
	[System.Xml.Serialization.XmlInclude(typeof(Method))]
	public abstract partial class TypeMember
	{		
		public static TypeMember Lift(TypeMember member)
		{
			return member;
		}

		public static TypeMember Lift(Statement stmt)
		{
			var typeMemberStatement = stmt as TypeMemberStatement;
			if (null != typeMemberStatement)
				return TypeMember.Lift(typeMemberStatement);

			var declaration = stmt as DeclarationStatement;
			if (null != declaration)
				return TypeMember.Lift(declaration);

			var expressionStatement = stmt as ExpressionStatement;
			if (null != expressionStatement)
				return TypeMember.Lift(expressionStatement);

			throw new NotImplementedException(stmt.ToCodeString());
		}

		public static TypeMember Lift(TypeMemberStatement stmt)
		{
			return stmt.TypeMember;
		}

		public static TypeMember Lift(DeclarationStatement stmt)
		{
			var closure = stmt.Initializer as BlockExpression;
			if (closure != null && closure.ContainsAnnotation(BlockExpression.ClosureNameAnnotation))
				return TypeMember.Lift(closure);

			return new Field(stmt.LexicalInfo)
			       	{
			       		Name = stmt.Declaration.Name,
						Type = stmt.Declaration.Type,
						Initializer = stmt.Initializer
			       	};
		}

		public static TypeMember Lift(ExpressionStatement stmt)
		{
			var e = stmt.Expression;
			var closure = e as BlockExpression;
			if (closure != null)
				return TypeMember.Lift(closure);

			throw new NotImplementedException(stmt.ToCodeString());
		}

		private static TypeMember Lift(BlockExpression closure)
		{
			return new Method(closure.LexicalInfo)
			        	{
							Name = (string)closure[BlockExpression.ClosureNameAnnotation],
							Parameters = closure.Parameters,
							ReturnType = closure.ReturnType,
							Body = closure.Body
			        	};
		}

		public static TypeMemberCollection Lift(Block block)
		{
			var members = new TypeMemberCollection();
			LiftBlockInto(members, block);
			return members;
		}

		private static void LiftBlockInto(TypeMemberCollection collection, Block block)
		{
			foreach (var stmt in block.Statements)
			{
				var childBlock = stmt as Block;
				if (childBlock != null)
					LiftBlockInto(collection, childBlock);
				else
					collection.Add(TypeMember.Lift(stmt));
			}
		}

		protected TypeMember()
		{
 		}
		
		protected TypeMember(TypeMemberModifiers modifiers, string name)
		{
			this.Modifiers = modifiers;
			this.Name = name;
		}		
		
		protected TypeMember(LexicalInfo lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public virtual TypeDefinition DeclaringType
		{
			get { return ParentNode as TypeDefinition; }
		}
		
		public virtual string FullName
		{
			get
			{
				if (null != ParentNode)
					return DeclaringType.FullName + "." + Name;
				return Name;
			}
		}
		
		public virtual NamespaceDeclaration EnclosingNamespace
		{
			get
			{
				Module enclosing = this.EnclosingModule;
				return enclosing == null ? null : enclosing.Namespace;
			}
		}
		
		public Module EnclosingModule
		{
			get { return GetAncestor<Module>(); }
		}

		public TypeMemberModifiers Visibility
		{
			get { return _modifiers & TypeMemberModifiers.VisibilityMask; }

			set
			{
				_modifiers &= ~TypeMemberModifiers.VisibilityMask;
				_modifiers |= value;
			}
		}

		public bool IsVisibilitySet
		{
			get { return IsPublic | IsInternal | IsPrivate | IsProtected; }
		}

		public bool IsVisible
		{
			get
			{
				if (IsPrivate || IsInternal)
					return false;

				TypeMember parent = DeclaringType;
				while (null != parent && !(parent is Module))
				{
					if (!parent.IsPublic)
						return false;
					parent = parent.DeclaringType;
				}

				return true;
			}
		}

		public bool IsAbstract
		{
			get { return IsModifierSet(TypeMemberModifiers.Abstract); }
		}
		
		public bool IsOverride
		{
			get { return IsModifierSet(TypeMemberModifiers.Override); }
		}
		
		public bool IsVirtual
		{
			get { return IsModifierSet(TypeMemberModifiers.Virtual); }
		}

		public bool IsNew
		{
			get { return IsModifierSet(TypeMemberModifiers.New); }
		}

		public virtual bool IsStatic
		{
			get { return IsModifierSet(TypeMemberModifiers.Static); }
		}
		
		public bool IsPublic
		{
			get { return IsModifierSet(TypeMemberModifiers.Public); }
		}
		
		public bool IsInternal
		{
			get { return IsModifierSet(TypeMemberModifiers.Internal); }
		}
		
		public bool IsProtected
		{
			get { return IsModifierSet(TypeMemberModifiers.Protected); }
		}
		
		public bool IsPrivate
		{
			get { return IsModifierSet(TypeMemberModifiers.Private); }
		}
		
		public bool IsFinal
		{
			get { return IsModifierSet(TypeMemberModifiers.Final); }
		}
		
		public bool IsTransient
		{
			get { return HasTransientModifier || IsStatic; }
		}

		public bool HasTransientModifier
		{
			get { return IsModifierSet(TypeMemberModifiers.Transient); }
		}

		public bool IsPartial
		{
			get { return IsModifierSet(TypeMemberModifiers.Partial); }
		}
		
		public bool IsModifierSet(TypeMemberModifiers modifiers)
		{
			return modifiers == (_modifiers & modifiers);
		}
		
		override public string ToString()
		{
			return FullName;
		}
	}
}
