using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class DeclarationStatementImpl : Statement
	{
		protected Declaration _declaration;
		protected Expression _initializer;
		
		protected DeclarationStatementImpl()
		{
 		}
		
		protected DeclarationStatementImpl(Declaration declaration, Expression initializer)
		{
 			Declaration = declaration;
			Initializer = initializer;
		}
		
		protected DeclarationStatementImpl(antlr.Token token, Declaration declaration, Expression initializer) : base(token)
		{
 			Declaration = declaration;
			Initializer = initializer;
		}
		
		internal DeclarationStatementImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal DeclarationStatementImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
 		}
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.DeclarationStatement;
			}
		}
		public Declaration Declaration
		{
			get
			{
				return _declaration;
			}
			
			set
			{
				_declaration = value;
				if (null != _declaration)
				{
					_declaration.InitializeParent(this);
				}
			}
		}
		public Expression Initializer
		{
			get
			{
				return _initializer;
			}
			
			set
			{
				_initializer = value;
				if (null != _initializer)
				{
					_initializer.InitializeParent(this);
				}
			}
		}
	}
}
