#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

namespace Boo.Lang.Compiler.Ast
{	
	public class AstUtil
	{
		public static bool IsListGenerator(Node node)
		{			
			if (NodeType.ListLiteralExpression == node.NodeType)
			{
				return IsListGenerator((ListLiteralExpression)node);
			}
			return false;
		}
		
		public static bool IsListGenerator(ListLiteralExpression node)
		{
			return 1 == node.Items.Count &&
				NodeType.GeneratorExpression == node.Items[0].NodeType;
		}		
		
		public static bool IsTargetOfMethodInvocation(Expression node)
		{
			return node.ParentNode.NodeType == NodeType.MethodInvocationExpression &&
					node == ((MethodInvocationExpression)node.ParentNode).Target;
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
		
		public static bool IsAssignmentOperator(BinaryOperatorType op)
		{
			return BinaryOperatorType.Assign == op ||
					BinaryOperatorType.InPlaceAdd == op ||
					BinaryOperatorType.InPlaceSubtract == op ||
					BinaryOperatorType.InPlaceMultiply == op ||
					BinaryOperatorType.InPlaceDivide == op;
		}
		
		public static Constructor CreateConstructor(Node lexicalInfoProvider, TypeMemberModifiers modifiers)
		{
			Constructor constructor = new Constructor(lexicalInfoProvider.LexicalInfo);
			constructor.Modifiers = modifiers;
			return constructor;
		}
		
		public static Expression CreateReferenceExpression(string fullname)
		{
			string[] parts = fullname.Split('.');
			ReferenceExpression expression = new ReferenceExpression(parts[0]);
			for (int i=1; i<parts.Length; ++i)
			{
				expression = new MemberReferenceExpression(expression, parts[i]);
			}
			return expression;
		}
		
		public static MethodInvocationExpression CreateMethodInvocationExpression(Expression target, Expression arg)
		{
			MethodInvocationExpression mie = new MethodInvocationExpression(arg.LexicalInfo);
			mie.Target = (Expression)target.Clone();			
			mie.Arguments.Add((Expression)arg.Clone());
			return mie;
		}
		
		private AstUtil()
		{
		}
	}
}
