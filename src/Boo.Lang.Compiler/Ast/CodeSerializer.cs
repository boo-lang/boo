using System;
using System.Collections.Generic;

namespace Boo.Lang.Compiler.Ast
{
	public partial class CodeSerializer : DepthFirstVisitor
	{
		private Stack<Expression> _stack = new Stack<Expression>();

		public Expression Serialize(AstLiteralExpression node)
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

		protected Expression SerializeCollection(string typeName, System.Collections.IEnumerable items)
		{
			MethodInvocationExpression mie = new MethodInvocationExpression(CreateReference(typeName + ".FromArray"));
			foreach (Node item in items)
			{
				mie.Arguments.Add(Serialize(item));
			}
			return mie;
		}

		public override void OnOmittedExpression(OmittedExpression node)
		{
			Push(CreateReference("Boo.Lang.Compiler.Ast.OmittedExpression.Default"));
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
