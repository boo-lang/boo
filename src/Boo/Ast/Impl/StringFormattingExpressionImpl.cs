using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class StringFormattingExpressionImpl : Expression
	{
		protected string _template;
		protected ExpressionCollection _arguments;
		
		protected StringFormattingExpressionImpl()
		{
			_arguments = new ExpressionCollection(this);
 		}
		
		protected StringFormattingExpressionImpl(string template)
		{
			_arguments = new ExpressionCollection(this);
 			Template = template;
		}
		
		protected StringFormattingExpressionImpl(antlr.Token token, string template) : base(token)
		{
			_arguments = new ExpressionCollection(this);
 			Template = template;
		}
		
		internal StringFormattingExpressionImpl(antlr.Token token) : base(token)
		{
			_arguments = new ExpressionCollection(this);
 		}
		
		internal StringFormattingExpressionImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
			_arguments = new ExpressionCollection(this);
 		}
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.StringFormattingExpression;
			}
		}
		public string Template
		{
			get
			{
				return _template;
			}
			
			set
			{
				
				_template = value;
			}
		}
		public ExpressionCollection Arguments
		{
			get
			{
				return _arguments;
			}
			
			set
			{
				
				if (_arguments != value)
				{
					_arguments = value;
					if (null != _arguments)
					{
						_arguments.InitializeParent(this);
					}
				}
			}
		}
		public override void Switch(IAstTransformer transformer, out Node resultingNode)
		{
			StringFormattingExpression thisNode = (StringFormattingExpression)this;
			Expression resultingTypedNode = thisNode;
			transformer.OnStringFormattingExpression(thisNode, ref resultingTypedNode);
			resultingNode = resultingTypedNode;
		}
	}
}
