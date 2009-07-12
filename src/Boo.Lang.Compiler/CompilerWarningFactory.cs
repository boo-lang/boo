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
using System.Text;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;

namespace Boo.Lang.Compiler
{
	public class CompilerWarningFactory
	{
		private CompilerWarningFactory()
		{
		}

		public static CompilerWarning CustomWarning(Node node, string msg)
		{
			return CustomWarning(AstUtil.SafeLexicalInfo(node), msg);
		}
		
		public static CompilerWarning CustomWarning(LexicalInfo lexicalInfo, string msg)
		{
			return new CompilerWarning(lexicalInfo, msg);
		}
		
		public static CompilerWarning CustomWarning(string msg)
		{
			return new CompilerWarning(msg);
		}
		
		public static CompilerWarning AbstractMemberNotImplemented(Node node, string typeName, string memberName)
		{
			return new CompilerWarning("BCW0001", AstUtil.SafeLexicalInfo(node), typeName, memberName);
		}
		
		public static CompilerWarning ModifiersInLabelsHaveNoEffect(Node node)
		{
			return new CompilerWarning("BCW0002", AstUtil.SafeLexicalInfo(node));
		}
		
		public static CompilerWarning UnusedLocalVariable(Node node, string name)
		{
			return new CompilerWarning("BCW0003", AstUtil.SafeLexicalInfo(node), name);
		}
		
		public static CompilerWarning IsInsteadOfIsa(Node node)
		{
			return new CompilerWarning("BCW0004", AstUtil.SafeLexicalInfo(node));
		}
		
		public static CompilerWarning InvalidEventUnsubscribe(Node node, string eventName, CallableSignature expected)
		{
			return new CompilerWarning("BCW0005", AstUtil.SafeLexicalInfo(node), eventName, expected);
		}

		public static CompilerWarning AssignmentToTemporary(Node node)
		{
			return new CompilerWarning("BCW0006", AstUtil.SafeLexicalInfo(node));
		}
		
		public static CompilerWarning EqualsInsteadOfAssign(BinaryExpression node)
		{
			return new CompilerWarning("BCW0007", AstUtil.SafeLexicalInfo(node), node.ToCodeString());
		}
		
		public static CompilerWarning DuplicateNamespace(Import import, string name)
		{
			return new CompilerWarning("BCW0008", AstUtil.SafeLexicalInfo(import), name);
		}
		
		public static CompilerWarning HaveBothKeyFileAndAttribute(Node node)
		{
			return new CompilerWarning("BCW0009", AstUtil.SafeLexicalInfo(node));
		}
		
		public static CompilerWarning HaveBothKeyNameAndAttribute(Node node)
		{
			return new CompilerWarning("BCW0010", AstUtil.SafeLexicalInfo(node));
		}

		public static CompilerWarning AbstractMemberNotImplementedStubCreated(Node node, string typeName, string memberName)
		{
			return new CompilerWarning("BCW0011", AstUtil.SafeLexicalInfo(node), typeName, memberName);
		}
		
		public static CompilerWarning Obsolete(Node node, string memberName, string message)
		{
			return new CompilerWarning("BCW0012", AstUtil.SafeLexicalInfo(node), memberName, message);
		}
		
		public static CompilerWarning StaticClassMemberRedundantlyMarkedStatic(Node node, string typeName, string memberName)
		{
			return new CompilerWarning("BCW0013", AstUtil.SafeLexicalInfo(node), typeName, memberName);
		}

		public static CompilerWarning PrivateMemberNeverUsed(TypeMember member)
		{
			return new CompilerWarning("BCW0014", AstUtil.SafeLexicalInfo(member), MemberVisibilityString(member), NodeTypeString(member), member.FullName);
		}

		public static CompilerWarning UnreachableCodeDetected(Node node)
		{
			return new CompilerWarning("BCW0015", AstUtil.SafeLexicalInfo(node));
		}

		public static CompilerWarning NamespaceNeverUsed(Import node)
		{
			return new CompilerWarning("BCW0016", AstUtil.SafeLexicalInfo(node), node.Namespace);
		}

		public static CompilerWarning NewProtectedMemberInSealedType(TypeMember member)
		{
			return new CompilerWarning("BCW0017", AstUtil.SafeLexicalInfo(member), NodeTypeString(member), member.Name, member.DeclaringType.Name);
		}

		public static CompilerWarning OverridingFinalizeIsBadPractice(TypeMember member)
		{
			return new CompilerWarning("BCW0018", AstUtil.SafeLexicalInfo(member));
		}

		public static CompilerWarning AmbiguousExceptionName(ExceptionHandler node)
		{
			return new CompilerWarning("BCW0019", AstUtil.SafeLexicalInfo(node), node.Declaration.Name);
		}

		public static CompilerWarning AssignmentToSameVariable(BinaryExpression node)
		{
			return new CompilerWarning("BCW0020", AstUtil.SafeLexicalInfo(node));
		}

		public static CompilerWarning ComparisonWithSameVariable(BinaryExpression node)
		{
			return new CompilerWarning("BCW0021", AstUtil.SafeLexicalInfo(node));
		}

		public static CompilerWarning ConstantExpression(Expression node)
		{
			return new CompilerWarning("BCW0022", AstUtil.SafeLexicalInfo(node));
		}

		public static CompilerWarning ImplicitReturn(Method node)
		{
			return new CompilerWarning("BCW0023", AstUtil.SafeLexicalInfo(node));
		}

		public static CompilerWarning VisibleMemberDoesNotDeclareTypeExplicitely(TypeMember node)
		{
			return VisibleMemberDoesNotDeclareTypeExplicitely(node, null);
		}

		public static CompilerWarning VisibleMemberDoesNotDeclareTypeExplicitely(TypeMember node, string argument)
		{
			string details = (null == argument)
				? ResourceManager.Format("BooC.Return")
				: ResourceManager.Format("BooC.NamedArgument", argument);
			return new CompilerWarning("BCW0024", AstUtil.SafeLexicalInfo(node), NodeTypeString(node), details);
		}

		public static CompilerWarning AmbiguousVariableName(Local node, string localName, string baseName)
		{
			return new CompilerWarning("BCW0025", AstUtil.SafeLexicalInfo(node), localName, baseName);
		}

		public static CompilerWarning LikelyTypoInTypeMemberName(TypeMember node, string suggestion)
		{
			return new CompilerWarning("BCW0026", AstUtil.SafeLexicalInfo(node), node.Name, suggestion);
		}

		public static CompilerWarning ObsoleteSyntax(Node anchor, string obsoleteSyntax, string newSyntax)
		{
			return new CompilerWarning("BCW0027", AstUtil.SafeLexicalInfo(anchor), obsoleteSyntax, newSyntax);
		}

		public static CompilerWarning ImplicitDowncast(Node node, IType lhs, IType rhs)
		{
			return new CompilerWarning("BCW0028", AstUtil.SafeLexicalInfo(node), lhs, rhs);
		}

		private static string NodeTypeString(Node node)
		{
			return node.NodeType.ToString().ToLower();
		}

		private static string MemberVisibilityString(TypeMember member)
		{
			switch (member.Modifiers & TypeMemberModifiers.VisibilityMask)
			{
				case TypeMemberModifiers.Private:
					return "Private";
				case TypeMemberModifiers.Internal:
					return "Internal";
				case TypeMemberModifiers.Protected:
					return "Protected";
			}
			return "Public";
		}
	}

}
