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
	using System.Xml.Serialization;
	
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

		public static bool IsAssignment(Expression node)
		{
			if (node.NodeType == NodeType.BinaryExpression)
			{
				BinaryOperatorType binaryOperator = ((BinaryExpression)node).Operator;
				return IsAssignmentOperator(binaryOperator);
			}
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
			return node.ParentNode.NodeType == NodeType.MethodInvocationExpression &&
					node == ((MethodInvocationExpression)node.ParentNode).Target;
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
				{
					return IsAssignmentOperator(be.Operator);
				}
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
		
		public static bool IsAssignmentOperator(BinaryOperatorType op)
		{
			return BinaryOperatorType.Assign == op ||
					BinaryOperatorType.InPlaceAddition == op ||
					BinaryOperatorType.InPlaceSubtraction == op ||
					BinaryOperatorType.InPlaceMultiply == op ||
					BinaryOperatorType.InPlaceDivision == op ||
					BinaryOperatorType.InPlaceBitwiseAnd == op ||
					BinaryOperatorType.InPlaceBitwiseOr == op ||
					BinaryOperatorType.InPlaceShiftLeft == op ||
					BinaryOperatorType.InPlaceShiftRight == op;
		}
		
		public static Constructor CreateConstructor(Node lexicalInfoProvider, TypeMemberModifiers modifiers)
		{
			Constructor constructor = new Constructor(lexicalInfoProvider.LexicalInfo);
			constructor.Modifiers = modifiers;
			constructor.IsSynthetic = true;
			return constructor;
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
			System.Console.WriteLine("{0}: {1} - {2}",
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
	}
}
