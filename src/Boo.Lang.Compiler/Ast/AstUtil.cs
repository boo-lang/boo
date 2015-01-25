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

using System.Linq;
using Boo.Lang.Environments;

namespace Boo.Lang.Compiler.Ast
{
	using System;
	using System.IO;
	using System.Text;
	using System.Xml.Serialization;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;

	public static class AstUtil
	{
		public static bool IsOverloadableOperator(BinaryOperatorType op)
		{
			switch (op)
			{
				case BinaryOperatorType.Addition:
				case BinaryOperatorType.Subtraction:
				case BinaryOperatorType.Multiply:
				case BinaryOperatorType.Division:
				case BinaryOperatorType.Modulus:
				case BinaryOperatorType.Exponentiation:
				case BinaryOperatorType.LessThan:
				case BinaryOperatorType.LessThanOrEqual:
				case BinaryOperatorType.GreaterThan:
				case BinaryOperatorType.GreaterThanOrEqual:
				case BinaryOperatorType.Match:
				case BinaryOperatorType.NotMatch:
				case BinaryOperatorType.Member:
				case BinaryOperatorType.NotMember:
				case BinaryOperatorType.BitwiseOr:
				case BinaryOperatorType.BitwiseAnd:
				case BinaryOperatorType.ExclusiveOr:
				case BinaryOperatorType.ShiftLeft:
				case BinaryOperatorType.ShiftRight:
				{
					return true;
				}
			}
			return false;
		}

		public static string GetMethodNameForOperator(BinaryOperatorType op)
		{
			return "op_" + op;
		}
		
		public static string GetMethodNameForOperator(UnaryOperatorType op)
		{
			return "op_" + op;
		}

		public static Node GetMemberAnchor(Node node)
		{
			var member = node as MemberReferenceExpression;
			return member != null ? member.Target : node;
		}

		public static bool IsPostUnaryOperator(UnaryOperatorType op)
		{
			return UnaryOperatorType.PostIncrement == op ||
				UnaryOperatorType.PostDecrement == op ||
				UnaryOperatorType.SafeAccess == op;
		}

		public static bool IsIncDec(Node node)
		{
			if (node.NodeType == NodeType.UnaryExpression)
			{
				UnaryOperatorType op = ((UnaryExpression)node).Operator;
				return UnaryOperatorType.Increment == op ||
					UnaryOperatorType.PostIncrement == op ||
					UnaryOperatorType.Decrement == op ||
					UnaryOperatorType.PostDecrement == op;
			}
			return false;
		}

		public static BinaryOperatorKind GetBinaryOperatorKind(BinaryExpression expression)
		{
			return GetBinaryOperatorKind(expression.Operator, false);
		}

		public static BinaryOperatorKind GetBinaryOperatorKind(BinaryExpression expression, bool exact)
		{
			return GetBinaryOperatorKind(expression.Operator, exact);
		}

		public static BinaryOperatorKind GetBinaryOperatorKind(BinaryOperatorType op)
		{
			return GetBinaryOperatorKind(op, false);
		}

		public static BinaryOperatorKind GetBinaryOperatorKind(BinaryOperatorType op, bool exact)
		{
			switch (op) {
				case BinaryOperatorType.Addition:
				case BinaryOperatorType.Subtraction:
				case BinaryOperatorType.Multiply:
				case BinaryOperatorType.Division:
				case BinaryOperatorType.Modulus:
				case BinaryOperatorType.Exponentiation:
					return BinaryOperatorKind.Arithmetic;

				case BinaryOperatorType.LessThan:
				case BinaryOperatorType.LessThanOrEqual:
				case BinaryOperatorType.GreaterThan:
				case BinaryOperatorType.GreaterThanOrEqual:
				case BinaryOperatorType.Equality:
				case BinaryOperatorType.Inequality:
				case BinaryOperatorType.Match:
				case BinaryOperatorType.NotMatch:
				case BinaryOperatorType.ReferenceEquality:
				case BinaryOperatorType.ReferenceInequality:
					return BinaryOperatorKind.Comparison;

				case BinaryOperatorType.TypeTest:
				case BinaryOperatorType.Member:
				case BinaryOperatorType.NotMember:
					return exact
						? BinaryOperatorKind.TypeComparison
						: BinaryOperatorKind.Comparison;

				case BinaryOperatorType.Assign:
					return BinaryOperatorKind.Assignment;

				case BinaryOperatorType.InPlaceAddition:
				case BinaryOperatorType.InPlaceSubtraction:
				case BinaryOperatorType.InPlaceMultiply:
				case BinaryOperatorType.InPlaceDivision:
				case BinaryOperatorType.InPlaceModulus:
				case BinaryOperatorType.InPlaceBitwiseAnd:
				case BinaryOperatorType.InPlaceBitwiseOr:
				case BinaryOperatorType.InPlaceExclusiveOr:
				case BinaryOperatorType.InPlaceShiftLeft:
				case BinaryOperatorType.InPlaceShiftRight:
					return exact
						? BinaryOperatorKind.InPlaceAssignment
						: BinaryOperatorKind.Assignment;

				case BinaryOperatorType.Or:
				case BinaryOperatorType.And:
					return BinaryOperatorKind.Logical;

				case BinaryOperatorType.BitwiseOr:
				case BinaryOperatorType.BitwiseAnd:
				case BinaryOperatorType.ExclusiveOr:
				case BinaryOperatorType.ShiftLeft:
				case BinaryOperatorType.ShiftRight:
					return BinaryOperatorKind.Bitwise;
			}
			throw new NotSupportedException(String.Format("unknown operator: {0}", op));
		}

		public static bool IsAssignment(Node node)
		{
			if (node.NodeType == NodeType.BinaryExpression)
				return GetBinaryOperatorKind((BinaryExpression) node) == BinaryOperatorKind.Assignment;
			return false;
		}

		public static ClassDefinition GetParentClass(Node node)
		{
			return (ClassDefinition) node.GetAncestor(NodeType.ClassDefinition);
		}
		
		public static Node GetParentTryExceptEnsure(Node node)
		{
			Node parent = node.ParentNode;
			while (null != parent)
			{
				switch (parent.NodeType)
				{
					case NodeType.TryStatement:
					case NodeType.ExceptionHandler:
					{
						return parent;
					}
					
					case NodeType.Block:
					{
						if (NodeType.TryStatement == parent.ParentNode.NodeType)
						{
							if (parent == ((TryStatement)parent.ParentNode).EnsureBlock)
							{
								return parent;
							}
						}
						break;
					}
					
					case NodeType.Method:
					{
						return null;
					}
				}
				parent = parent.ParentNode;
			}
			return null;
		}
		
		public static bool IsListGenerator(Node node)
		{			
			return NodeType.ListLiteralExpression == node.NodeType
				? IsListGenerator((ListLiteralExpression)node)
				: false;
		}
		
		public static bool IsListGenerator(ListLiteralExpression node)
		{
			if (1 == node.Items.Count)
			{
				NodeType itemType = node.Items[0].NodeType;
				return NodeType.GeneratorExpression == itemType;
			}
			return false;
		}
		
		public static bool IsListMultiGenerator(Node node)
		{			
			return NodeType.ListLiteralExpression == node.NodeType
				? IsListMultiGenerator((ListLiteralExpression)node)
				: false;
		}

		public static bool IsListMultiGenerator(ListLiteralExpression node)
		{
			if (1 == node.Items.Count)
			{
				NodeType itemType = node.Items[0].NodeType;
				return NodeType.ExtendedGeneratorExpression == itemType;
			}
			return false;
		}		
		
		public static bool IsTargetOfMethodInvocation(Expression node)
		{
			return IsTargetOfGenericMethodInvocation(node) ||
				(node.ParentNode.NodeType == NodeType.MethodInvocationExpression &&
					node == ((MethodInvocationExpression)node.ParentNode).Target);
		}

		public static bool IsTargetOfGenericMethodInvocation(Expression node)
		{
            return node.ParentNode.NodeType == NodeType.GenericReferenceExpression && node.ParentNode.ParentNode != null
                    && node.ParentNode.ParentNode.NodeType == NodeType.MethodInvocationExpression
                    && node.ParentNode == ((MethodInvocationExpression)node.ParentNode.ParentNode).Target;
		}

		public static bool IsTargetOfMemberReference(Expression node)
		{
			return node.ParentNode.NodeType == NodeType.MemberReferenceExpression &&
				node == ((MemberReferenceExpression)node.ParentNode).Target;
		}
		
		public static bool IsTargetOfSlicing(Expression node)
		{
			if (NodeType.SlicingExpression == node.ParentNode.NodeType)
			{
				if (node == ((SlicingExpression)node.ParentNode).Target)
				{
					return true;
				}
			}
			return false;
		}
		
		[Obsolete("Use node.IsTargetOfAssignment()")]
		public static bool IsLhsOfAssignment(Expression node)
		{
			return node.IsTargetOfAssignment();
		}

		public static bool IsRhsOfAssignment(Expression node)
		{
			var parentExpression = node.ParentNode as BinaryExpression;
			if (parentExpression == null)
				return false;
			return node == parentExpression.Right && IsAssignment(parentExpression);
		}
		
		public static bool IsLhsOfInPlaceAddSubtract(Expression node)
		{
			if (NodeType.BinaryExpression == node.ParentNode.NodeType)
			{
				var be = (BinaryExpression)node.ParentNode;
				if (node == be.Left)
				{
					var op = be.Operator;
					return op == BinaryOperatorType.InPlaceAddition
						|| op == BinaryOperatorType.InPlaceSubtraction;
				}
			}
			return false;
		}
		
		public static bool IsStandaloneReference(Node node)
		{
			Node parent = node.ParentNode;
			if (parent is GenericReferenceExpression)
				parent = parent.ParentNode;
			return parent.NodeType != NodeType.MemberReferenceExpression;
		}

		public static Constructor CreateConstructor(Node lexicalInfoProvider, TypeMemberModifiers modifiers)
		{
			return new Constructor(lexicalInfoProvider.LexicalInfo) { Modifiers = modifiers, IsSynthetic = true };
		}

		public static Constructor CreateDefaultConstructor(TypeDefinition type)
		{
			TypeMemberModifiers modifiers = TypeMemberModifiers.Public;
			if (type.IsAbstract)
				modifiers = TypeMemberModifiers.Protected;
			return CreateConstructor(type, modifiers);
		}

		public static ReferenceExpression CreateReferenceExpression(LexicalInfo li, string fullname)
		{
			var e = CreateReferenceExpression(fullname);
			e.LexicalInfo = li;
			return e;
		}
		
		public static ReferenceExpression CreateReferenceExpression(string fullname)
		{
			var parts = fullname.Split('.');
			var expression = new ReferenceExpression(parts[0]) {IsSynthetic = true};
		    for (var i=1; i<parts.Length; ++i)
				expression = new MemberReferenceExpression(expression, parts[i]) {IsSynthetic = true};
			return expression;
		}
		
		public static MethodInvocationExpression CreateMethodInvocationExpression(Expression target, Expression arg)
		{
			return CreateMethodInvocationExpression(arg.LexicalInfo, target, arg);
		}
		
		public static MethodInvocationExpression CreateMethodInvocationExpression(LexicalInfo li, Expression target, Expression arg)
		{
			var mie = new MethodInvocationExpression(li) { Target = (Expression) target.Clone(), IsSynthetic = true };
			mie.Arguments.Add((Expression)arg.Clone());
			return mie;
		}

		public static bool IsExplodeExpression(Node node)
		{
			var e = node as UnaryExpression;
			return e == null ? false : e.Operator == UnaryOperatorType.Explode;
		}

		public static bool IsIndirection(Node node)
		{
			var e = node as UnaryExpression;
			return e == null ? false : e.Operator == UnaryOperatorType.Indirection;
		}

		public static string ToXml(Node node)
		{
			var writer = new StringWriter();
			new XmlSerializer(node.GetType()).Serialize(writer, node);
			return writer.ToString();
		}

		public static Node FromXml(Type type, string code)
		{
			return (Node)new XmlSerializer(type).Deserialize(new StringReader(code));
		}
		
		public static void DebugNode(Node node)
		{
			Console.WriteLine("{0}: {1} - {2}",
					node.LexicalInfo,
					node.NodeType,
					SafeToCodeString(node));
		}
		
		public static string SafeToCodeString(Node node)
		{
			try
			{
				return node.ToCodeString();
			}
			catch (Exception)
			{
				return "<unavailable>";
			}
		}

		//use this to build a type member name unique in the inheritance hierarchy.
		public static string BuildUniqueTypeMemberName(TypeDefinition type, string name)
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");

			var nameBuilder = new StringBuilder("$");
			nameBuilder.Append(name);
			nameBuilder.Append("__");
			nameBuilder.Append(type.QualifiedName);
			if (type.HasGenericParameters)
			{
				nameBuilder.Append("_");

				foreach (var parameterName in type.GenericParameters.Select(gpd => gpd.Name))
				{
					nameBuilder.Append("_");
					nameBuilder.Append(parameterName);
				}
			}
			nameBuilder.Replace('.', '_');
			nameBuilder.Append("$");
			return nameBuilder.ToString();
		}

		internal static Local GetLocalByName(Method method, string name)
		{
			if (method.Locals.Count == 0)
				return null;

			return method.Locals.FirstOrDefault(local => !local.PrivateScope && local.Name == name);
		}

		public static LexicalInfo SafeLexicalInfo(Node node)
		{
			if (node == null)
				return LexicalInfo.Empty;
			var info = node.LexicalInfo;
			return info.IsValid ? info : SafeLexicalInfo(node.ParentNode);
		}

		public static string SafePositionOnlyLexicalInfo(Node node)
		{
			var info = SafeLexicalInfo(node);
			return string.Format("({0},{1})", info.Line, info.Column);
		}

		public static ICollection<TValue> GetValues<TNode, TValue>(NodeCollection<TNode> nodes)
			where TNode : LiteralExpression
		{
			return nodes.Select(node => (TValue) Convert.ChangeType(node.ValueObject, typeof(TValue))).ToList();
		}

		internal static bool AllCodePathsReturnOrRaise(Block block)
		{
			if (block == null || block.IsEmpty)
				return false;

			var node = block.LastStatement;
			var last = node.NodeType;
			switch (last)
			{
				case NodeType.ReturnStatement:
				case NodeType.RaiseStatement:
					return true;

				case NodeType.Block:
					return AllCodePathsReturnOrRaise((Block)node);

				case NodeType.IfStatement:
					var ifstmt = (IfStatement) node;
					return
						AllCodePathsReturnOrRaise(ifstmt.TrueBlock)
						&& AllCodePathsReturnOrRaise(ifstmt.FalseBlock);

				case NodeType.TryStatement:
					var ts = (TryStatement) node;
					if (!AllCodePathsReturnOrRaise(ts.ProtectedBlock))
						return false;
					//if (null != ts.FailureBlock && !EndsWithReturnStatement(ts.FailureBlock))
					//	return false;
					return ts.ExceptionHandlers.Select(handler => handler.Block).All(AllCodePathsReturnOrRaise);
			}
			return false;
		}

		internal static RegexOptions GetRegexOptions(RELiteralExpression re)
		{
			RegexOptions ro = RegexOptions.None;

			if (String.IsNullOrEmpty(re.Options))
				return ro;

			foreach (char opt in re.Options)
			{
				switch (opt)
				{
					/* common flags */
					case 'g':
						break; //no-op on .NET, global by default
					case 'i':
						ro |= RegexOptions.IgnoreCase;
						break;
					case 'm':
						ro |= RegexOptions.Multiline;
						break;

					/* perl|python flags */
					case 's':
						ro |= RegexOptions.Singleline;
						break;
					case 'x':
						//TODO: parser support, no-op for now
						//ro |= RegexOptions.IgnorePatternWhitespace;
						break;
					case 'l': //no-op on .NET, (l)ocale-aware by default
						break;

					/* boo-specific flags */
					case 'n': //culture-(n)eutral
						ro |= RegexOptions.CultureInvariant;
						break;
					case 'c':
						#if !MONOTOUCH
						ro |= RegexOptions.Compiled;
						#endif
						break;
					case 'e':
						ro |= RegexOptions.ExplicitCapture;
						break;

					default:
						My<CompilerErrorCollection>.Instance.Add(
							CompilerErrorFactory.InvalidRegexOption(re, opt));
						break;
				}
			}
			return ro;
		}

		public static bool IsTargetOfGenericReferenceExpression(Expression node)
		{
			var parent = node.ParentNode as GenericReferenceExpression;
			if (parent == null)
				return false;
			return parent.Target == node;
		}

		public static bool InvocationEndsWithExplodeExpression(MethodInvocationExpression node)
		{
			return EndsWithExplodeExpression(node.Arguments);
		}

		public static bool EndsWithExplodeExpression(ExpressionCollection expressionCollection)
		{
			return expressionCollection.Count > 0 && IsExplodeExpression(expressionCollection[-1]);
		}

		public static string TypeKeywordFor(TypeDefinition node)
		{
			switch (node.NodeType)
			{
				case NodeType.ClassDefinition:
					return "class";
				case NodeType.InterfaceDefinition:
					return "interface";
				case NodeType.StructDefinition:
					return "struct";
				case NodeType.EnumDefinition:
					return "enum";
				default:
					throw new ArgumentException("Unsupported type definition kind: " + node.NodeType, "node");
			}
		}
	}
}

