using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class UnpackStatementImpl : Statement
	{
		protected DeclarationCollection _declarations;
		protected Expression _expression;
		
		protected UnpackStatementImpl()
		{
			_declarations = new DeclarationCollection(this);
 		}
		
		protected UnpackStatementImpl(Expression expression)
		{
			_declarations = new DeclarationCollection(this);
 			Expression = expression;
		}
		
		protected UnpackStatementImpl(antlr.Token token, Expression expression) : base(token)
		{
			_declarations = new DeclarationCollection(this);
 			Expression = expression;
		}
		
		internal UnpackStatementImpl(antlr.Token token) : base(token)
		{
			_declarations = new DeclarationCollection(this);
 		}
		
		internal UnpackStatementImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
			_declarations = new DeclarationCollection(this);
 		}
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.UnpackStatement;
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
				
				if (_declarations != value)
				{
					_declarations = value;
					if (null != _declarations)
					{
						_declarations.InitializeParent(this);
					}
				}
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
				
				if (_expression != value)
				{
					_expression = value;
					if (null != _expression)
					{
						_expression.InitializeParent(this);
					}
				}
			}
		}
		public override void Switch(IAstTransformer transformer, out Node resultingNode)
		{
			UnpackStatement thisNode = (UnpackStatement)this;
			Statement resultingTypedNode = thisNode;
			transformer.OnUnpackStatement(thisNode, ref resultingTypedNode);
			resultingNode = resultingTypedNode;
		}
	}
}
