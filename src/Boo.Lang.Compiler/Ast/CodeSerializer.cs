using System;
using System.Collections.Generic;

namespace Boo.Lang.Compiler.Ast
{
	public partial class CodeSerializer : DepthFirstVisitor
	{
		private Stack<Expression> _stack = new Stack<Expression>();

		public Expression Serialize(QuasiquoteExpression node)
		{
			Expression e = Serialize(node.Node);
			if (_stack.Count != 0) throw new InvalidOperationException();
			return e;
		}

		public Expression Serialize(Node node)
		{
			if (null == node) return new NullLiteralExpression();
			node.Accept(this);
			return _stack.Pop();
		}

		public Expression CreateReference(Node sourceNode, string qname)
		{
			return AstUtil.CreateReferenceExpression(sourceNode.LexicalInfo, qname);
		}
		
		public Expression CreateReference(string qname)
		{
			return AstUtil.CreateReferenceExpression(qname);
		}

		public bool ShouldSerialize<T>(NodeCollection<T> c) where T: Node
		{
			return c.Count > 0;
		}

		public bool ShouldSerialize(object value)
		{
			return value != null;
		}

		public Expression Serialize(string value)
		{
			return new StringLiteralExpression(value);
		}

		public Expression Serialize(bool value)
		{
			return new BoolLiteralExpression(value);
		}

		public Expression Serialize(long value)
		{
			return new IntegerLiteralExpression(value);
		}

		public Expression Serialize(double value)
		{
			return new DoubleLiteralExpression(value);
		}

		public Expression Serialize(TimeSpan value)
		{
			return new TimeSpanLiteralExpression(value);
		}

		private Expression SerializeEnum(string enumType, long value)
		{	
			return new CastExpression(
				Serialize(value),
				new SimpleTypeReference("Boo.Lang.Compiler.Ast." + enumType));
		}
		
		protected Expression SerializeCollection(Node sourceNode, string typeName, StatementCollection items)
		{
			MethodInvocationExpression mie = CreateFromArrayInvocation(sourceNode, typeName);
			foreach (Statement item in items)
			{
				mie.Arguments.Add(LiftStatement(Serialize(item)));
			}
			return mie;
		}
		
		private MethodInvocationExpression CreateFromArrayInvocation(Node sourceNode, string typeName)
		{
			return new MethodInvocationExpression(
							sourceNode.LexicalInfo,
							CreateReference(sourceNode, typeName + ".FromArray"));
		}

		protected Expression SerializeCollection(Node sourceNode, string typeName, System.Collections.IEnumerable items)
		{
			MethodInvocationExpression mie = CreateFromArrayInvocation(sourceNode, typeName);
			foreach (Node item in items)
			{
				mie.Arguments.Add(Serialize(item));
			}
			return mie;
		}
		
		public override void OnExpressionStatement(ExpressionStatement node)
		{
			Visit(node.Expression);
		}

		public override void OnOmittedExpression(OmittedExpression node)
		{
			Push(CreateReference(node, "Boo.Lang.Compiler.Ast.OmittedExpression.Default"));
		}

		public override void OnSpliceExpression(SpliceExpression node)
		{
			if (IsStatementExpression(node))
			{
				Push(LiftStatement(node.Expression));
				return;
			}

			Push(LiftExpression(node.Expression));
		}

		private MethodInvocationExpression LiftStatement(Expression node)
		{
			return Lift("Boo.Lang.Compiler.Ast.Statement.Lift", node);
		}

		private MethodInvocationExpression LiftExpression(Expression node)
		{
			return Lift("Boo.Lang.Compiler.Ast.Expression.Lift", node);
		}

		private MethodInvocationExpression Lift(string methodName, Expression node)
		{
			MethodInvocationExpression lift = CreateInvocation(node, methodName);
			lift.Arguments.Add(node);
			return lift;
		}

		private MethodInvocationExpression CreateInvocation(Node sourceNode, string reference)
		{
			return new MethodInvocationExpression(sourceNode.LexicalInfo, CreateReference(sourceNode, reference));
		}

		private static bool IsStatementExpression(SpliceExpression node)
		{
			return node.ParentNode.NodeType == NodeType.ExpressionStatement;
		}

		private void Push(Expression node)
		{
			_stack.Push(node);
		}

		private Expression Pop()
		{
			return _stack.Pop();
		}
	}
}
