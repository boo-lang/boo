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
		
		public StatementCollection Statements
		{
			get
			{
				return _statements;
			}
			
			set
			{
				_statements = value;
				if (null != _statements)
				{
					_statements.InitializeParent(this);
				}
			}
		}
	}
}
