using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class MethodInvocationExpressionImpl : Expression, INodeWithArguments
	{
		protected Expression _target;
		protected ExpressionCollection _arguments;
		protected ExpressionPairCollection _namedArguments;
		
		protected MethodInvocationExpressionImpl()
		{
			_arguments = new ExpressionCollection(this);
			_namedArguments = new ExpressionPairCollection(this);
 		}
		
		protected MethodInvocationExpressionImpl(Expression target)
		{
			_arguments = new ExpressionCollection(this);
			_namedArguments = new ExpressionPairCollection(this);
 			Target = target;
		}
		
		protected MethodInvocationExpressionImpl(antlr.Token token, Expression target) : base(token)
		{
			_arguments = new ExpressionCollection(this);
			_namedArguments = new ExpressionPairCollection(this);
 			Target = target;
		}
		
		internal MethodInvocationExpressionImpl(antlr.Token token) : base(token)
		{
			_arguments = new ExpressionCollection(this);
			_namedArguments = new ExpressionPairCollection(this);
 		}
		
		internal MethodInvocationExpressionImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
			_arguments = new ExpressionCollection(this);
			_namedArguments = new ExpressionPairCollection(this);
 		}
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.MethodInvocationExpression;
			}
		}
		public Expression Target
		{
			get
			{
				return _target;
			}
			
			set
			{
				
				if (_target != value)
				{
					_target = value;
					if (null != _target)
					{
						_target.InitializeParent(this);
					}
				}
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
		public ExpressionPairCollection NamedArguments
		{
			get
			{
				return _namedArguments;
			}
			
			set
			{
				
				if (_namedArguments != value)
				{
					_namedArguments = value;
					if (null != _namedArguments)
					{
						_namedArguments.InitializeParent(this);
					}
				}
			}
		}
		public override void Switch(IAstTransformer transformer, out Node resultingNode)
		{
			MethodInvocationExpression thisNode = (MethodInvocationExpression)this;
			Expression resultingTypedNode = thisNode;
			transformer.OnMethodInvocationExpression(thisNode, ref resultingTypedNode);
			resultingNode = resultingTypedNode;
		}
	}
}
