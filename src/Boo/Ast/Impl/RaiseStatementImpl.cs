using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class RaiseStatementImpl : Statement
	{
		protected Expression _exception;
		
		protected RaiseStatementImpl()
		{
 		}
		
		protected RaiseStatementImpl(Expression exception)
		{
 			Exception = exception;
		}
		
		protected RaiseStatementImpl(antlr.Token token, Expression exception) : base(token)
		{
 			Exception = exception;
		}
		
		internal RaiseStatementImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal RaiseStatementImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
 		}
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.RaiseStatement;
			}
		}
		public Expression Exception
		{
			get
			{
				return _exception;
			}
			
			set
			{
				
				if (_exception != value)
				{
					_exception = value;
					if (null != _exception)
					{
						_exception.InitializeParent(this);
					}
				}
			}
		}
		public override void Switch(IAstTransformer transformer, out Node resultingNode)
		{
			RaiseStatement thisNode = (RaiseStatement)this;
			Statement resultingTypedNode = thisNode;
			transformer.OnRaiseStatement(thisNode, ref resultingTypedNode);
			resultingNode = resultingTypedNode;
		}
	}
}
