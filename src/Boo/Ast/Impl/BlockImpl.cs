using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class BlockImpl : Node
	{
		protected StatementCollection _statements;
		
		protected BlockImpl()
		{
			_statements = new StatementCollection(this);
 		}
		
		internal BlockImpl(antlr.Token token) : base(token)
		{
			_statements = new StatementCollection(this);
 		}
		
		internal BlockImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
			_statements = new StatementCollection(this);
 		}
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.Block;
			}
		}
		public StatementCollection Statements
		{
			get
			{
				return _statements;
			}
			
			set
			{
				
				if (_statements != value)
				{
					_statements = value;
					if (null != _statements)
					{
						_statements.InitializeParent(this);
					}
				}
			}
		}
		public override void Switch(IAstTransformer transformer, out Node resultingNode)
		{
			Block thisNode = (Block)this;
			Block resultingTypedNode = thisNode;
			transformer.OnBlock(thisNode, ref resultingTypedNode);
			resultingNode = resultingTypedNode;
		}
	}
}
