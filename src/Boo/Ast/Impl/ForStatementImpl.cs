using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class ForStatementImpl : Statement
	{
		protected DeclarationCollection _declarations;
		protected Expression _iterator;
		protected StatementCollection _statements;
		
		protected ForStatementImpl()
		{
			_declarations = new DeclarationCollection(this);
			_statements = new StatementCollection(this);
 		}
		
		protected ForStatementImpl(Expression iterator)
		{
			_declarations = new DeclarationCollection(this);
			_statements = new StatementCollection(this);
 			Iterator = iterator;
		}
		
		protected ForStatementImpl(antlr.Token token, Expression iterator) : base(token)
		{
			_declarations = new DeclarationCollection(this);
			_statements = new StatementCollection(this);
 			Iterator = iterator;
		}
		
		internal ForStatementImpl(antlr.Token token) : base(token)
		{
			_declarations = new DeclarationCollection(this);
			_statements = new StatementCollection(this);
 		}
		
		internal ForStatementImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
			_declarations = new DeclarationCollection(this);
			_statements = new StatementCollection(this);
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
