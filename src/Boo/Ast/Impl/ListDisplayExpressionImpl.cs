using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class ListDisplayExpressionImpl : Expression
	{
		protected Expression _expression;
		protected DeclarationCollection _declarations;
		protected Expression _iterator;
		protected StatementModifier _filter;
		
		protected ListDisplayExpressionImpl()
		{
			_declarations = new DeclarationCollection(this);
 		}
		
		protected ListDisplayExpressionImpl(Expression expression, Expression iterator, StatementModifier filter)
		{
			_declarations = new DeclarationCollection(this);
 			Expression = expression;
			Iterator = iterator;
			Filter = filter;
		}
		
		protected ListDisplayExpressionImpl(antlr.Token token, Expression expression, Expression iterator, StatementModifier filter) : base(token)
		{
			_declarations = new DeclarationCollection(this);
 			Expression = expression;
			Iterator = iterator;
			Filter = filter;
		}
		
		internal ListDisplayExpressionImpl(antlr.Token token) : base(token)
		{
			_declarations = new DeclarationCollection(this);
 		}
		
		internal ListDisplayExpressionImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
			_declarations = new DeclarationCollection(this);
 		}
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.ListDisplayExpression;
			}
		}
		public Expression Expression
		{
			get
			{
				return _expression;
			}
			
			set
			{
				_expression = value;
				if (null != _expression)
				{
					_expression.InitializeParent(this);
				}
			}
		}
		public DeclarationCollection Declarations
		{
			get
			{
				return _declarations;
			}
			
			set
			{
				_declarations = value;
				if (null != _declarations)
				{
					_declarations.InitializeParent(this);
				}
			}
		}
		public Expression Iterator
		{
			get
			{
				return _iterator;
			}
			
			set
			{
				_iterator = value;
				if (null != _iterator)
				{
					_iterator.InitializeParent(this);
				}
			}
		}
		public StatementModifier Filter
		{
			get
			{
				return _filter;
			}
			
			set
			{
				_filter = value;
				if (null != _filter)
				{
					_filter.InitializeParent(this);
				}
			}
		}
		public override void Switch(IAstTransformer transformer, out Node resultingNode)
		{
			Expression resultingTypedNode;
			transformer.OnListDisplayExpression((ListDisplayExpression)this, out resultingTypedNode);
			resultingNode = resultingTypedNode;
		}
	}
}
