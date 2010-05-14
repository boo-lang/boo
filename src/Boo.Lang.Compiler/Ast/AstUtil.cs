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

namespace Boo.Lang.Compiler.Ast
{
	using System;
	using System.IO;
	using System.Text;
	using System.Xml.Serialization;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;

	public class AstUtil
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
			return "op_" + op.ToString();
		}
		
		public static string GetMethodNameForOperator(UnaryOperatorType op)
		{
			return "op_" + op.ToString();
		}

		public static bool IsComplexSlicing(SlicingExpression node)
		{
			foreach (Slice slice in node.Indices)
			{
				if (IsComplexSlice(slice))
				{
					return true;
				}
			}
			return false;
		}
		
		public static bool IsComplexSlice(Slice slice)
		{
			return null != slice.End
				|| null != slice.Step
				|| OmittedExpression.Default == slice.Begin;
		}

		public static Node GetMemberAnchor(Node node)
		{
			MemberReferenceExpression member = node as MemberReferenceExpression;
			return member != null ? member.Target : node;
		}

		public static bool IsPostUnaryOperator(UnaryOperatorType op)
		{
			return UnaryOperatorType.PostIncrement == op ||
				UnaryOperatorType.PostDecrement == op;
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
		
		public static bool IsLhsOfAssignment(Expression node)
		{
			if (NodeType.BinaryExpression == node.ParentNode.NodeType)
			{
				BinaryExpression be = (BinaryExpression)node.ParentNode;
				if (node == be.Left)
					return IsAssignment(be);
			}
			return false;
		}
		
		public static bool IsLhsOfInPlaceAddSubtract(Expression node)
		{
			if (NodeType.BinaryExpression == node.ParentNode.NodeType)
			{
				BinaryExpression be = (BinaryExpression)node.ParentNode;
				if (node == be.Left)
				{
					BinaryOperatorType op = be.Operator;
					return op == BinaryOperatorType.InPlaceAddition ||
							op == BinaryOperatorType.InPlaceSubtraction;
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
			Constructor constructor = new Constructor(lexicalInfoProvider.LexicalInfo);
			constructor.Modifiers = modifiers;
			constructor.IsSynthetic = true;
			return constructor;
		}

		public static Constructor CreateDefaultConstructor(TypeDefinition type)
		{
			TypeMemberModifiers modifiers = TypeMemberModifiers.Public;
			if (type is Module)
				modifiers = TypeMemberModifiers.Private;
			if (type.IsAbstract)
				modifiers = TypeMemberModifiers.Protected;

			return CreateConstructor(type, modifiers);
		}

		public static ReferenceExpression CreateReferenceExpression(LexicalInfo li, string fullname)
		{
			ReferenceExpression e = CreateReferenceExpression(fullname);
			e.LexicalInfo = li;
			return e;
		}
		
		public static ReferenceExpression CreateReferenceExpression(string fullname)
		{
			string[] parts = fullname.Split('.');
			ReferenceExpression expression = new ReferenceExpression(parts[0]);
			expression.IsSynthetic = true;
			for (int i=1; i<parts.Length; ++i)
			{
				expression = new MemberReferenceExpression(expression, parts[i]);
				expression.IsSynthetic = true;
			}
			return expression;
		}
		
		public static MethodInvocationExpression CreateMethodInvocationExpression(Expression target, Expression arg)
		{
			return CreateMethodInvocationExpression(arg.LexicalInfo, target, arg);
		}
		
		public static MethodInvocationExpression CreateMethodInvocationExpression(LexicalInfo li, Expression target, Expression arg)
		{
			MethodInvocationExpression mie = new MethodInvocationExpression(li);
			mie.Target = (Expression)target.Clone();			
			mie.Arguments.Add((Expression)arg.Clone());
			mie.IsSynthetic = true;
			return mie;
		}

		public static bool IsExplodeExpression(Node node)
		{
			UnaryExpression e = node as UnaryExpression;
			return null == e ? false : e.Operator == UnaryOperatorType.Explode;
		}

		public static bool IsIndirection(Node node)
		{
			UnaryExpression e = node as UnaryExpression;
			return null == e ? false : e.Operator == UnaryOperatorType.Indirection;
		}

		private AstUtil()
		{
		}

		public static string ToXml(Node node)
		{
			StringWriter writer = new StringWriter();
			XmlSerializer serializer = new XmlSerializer(node.GetType());
			serializer.Serialize(writer, node);
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
			if (String.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");

			StringBuilder nameBuilder = new StringBuilder("$");
			nameBuilder.Append(name);
			nameBuilder.Append("__");
			nameBuilder.Append(type.QualifiedName);
			if (type.HasGenericParameters)
			{
				nameBuilder.Append("_");
				string[] parameterNames = Array.ConvertAll<GenericParameterDeclaration, string>(
					type.GenericParameters.ToArray(),
					delegate(GenericParameterDeclaration gpd) { return gpd.Name; });
				foreach (string parameterName in parameterNames)
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

			foreach (Local local in method.Locals) {
				if (!local.PrivateScope && local.Name == name)
					return local;
			}
			return null;
		}

		public static LexicalInfo SafeLexicalInfo(Node node)
		{
			if (null == node) return LexicalInfo.Empty;
			LexicalInfo info = node.LexicalInfo;
			if (info.IsValid) return info;
			return SafeLexicalInfo(node.ParentNode);
		}

		public static string SafePositionOnlyLexicalInfo(Node node)
		{
			LexicalInfo info = SafeLexicalInfo(node);
			return String.Format("({0},{1})", info.Line, info.Column);
		}

		public static ICollection<TValue> GetValues<TNode, TValue>(NodeCollection<TNode> nodes)
			where TNode : LiteralExpression
		{
			List<TValue> values = new List<TValue>(nodes.Count);

			foreach (TNode node in nodes)
				values.Add((TValue) Convert.ChangeType(node.ValueObject, typeof(TValue)));

			return values;
		}

		internal static bool AllCodePathsReturnOrRaise(Block block)
		{
			if (null == block || block.IsEmpty)
				return false;

			Node node = block.LastStatement;
			NodeType last = node.NodeType;
			switch (last)
			{
				case NodeType.ReturnStatement:
				case NodeType.RaiseStatement:
					return true;

				case NodeType.Block:
					return AllCodePathsReturnOrRaise((Block)node);

				case NodeType.IfStatement:
					IfStatement ifstmt = (IfStatement) node;
					return
						AllCodePathsReturnOrRaise(ifstmt.TrueBlock)
						&& AllCodePathsReturnOrRaise(ifstmt.FalseBlock);

				case NodeType.TryStatement:
					TryStatement ts = (TryStatement) node;
					if (!AllCodePathsReturnOrRaise(ts.ProtectedBlock))
						return false;
					//if (null != ts.FailureBlock && !EndsWithReturnStatement(ts.FailureBlock))
					//	return false;
					foreach (ExceptionHandler handler in ts.ExceptionHandlers)
					{
						if (!AllCodePathsReturnOrRaise(handler.Block))
							return false;
					}
					return true;
			}
			return false;
		}

		internal static RegexOptions GetRegexOptions(RELiteralExpression re)
		{
			RegexOptions ro = RegexOptions.None;

			if (string.IsNullOrEmpty(re.Options))
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
						CompilerContext.Current.Errors.Add(
							CompilerErrorFactory.InvalidRegexOption(re, opt));
						break;
				}
			}
			return ro;
		}
	}
}

