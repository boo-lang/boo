#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, 
// publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
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
			ListLiteralExpression lle = node as ListLiteralExpression;
			if (null != lle)
			{
				return IsListGenerator(lle);
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
				if (BinaryOperatorType.Assign == be.Operator &&
					node == be.Left)
				{
					return true;
				}
			}
			return false;
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
		
		private AstUtil()
		{
		}
	}
}
