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

		public Expression Serialize(StatementModifierType value)
		{
			return SerializeEnum("StatementModifierType", (long)value);
		}

		public Expression Serialize(ParameterModifiers value)
		{
			return SerializeEnum("ParameterModifiers", (long)value);
		}

		public Expression Serialize(BinaryOperatorType value)
		{
			return SerializeEnum("BinaryOperatorType", (long)value);
		}

		public Expression Serialize(UnaryOperatorType value)
		{
			return SerializeEnum("UnaryOperatorType", (long)value);
		}

		public Expression Serialize(TypeMemberModifiers value)
		{
			return SerializeEnum("TypeMemberModifiers", (long) value);
		}

		public Expression Serialize(MethodImplementationFlags value)
		{
			return SerializeEnum("MethodImplementationFlags", (long) value);
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
