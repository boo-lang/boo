using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class ExceptionHandlerImpl : Block
	{
		protected Declaration _declaration;
		
		protected ExceptionHandlerImpl()
		{
 		}
		
		protected ExceptionHandlerImpl(Declaration declaration)
		{
 			Declaration = declaration;
		}
		
		protected ExceptionHandlerImpl(antlr.Token token, Declaration declaration) : base(token)
		{
 			Declaration = declaration;
		}
		
		internal ExceptionHandlerImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal ExceptionHandlerImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
 		}
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.ExceptionHandler;
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
		public override void Switch(IAstTransformer transformer, out Node resultingNode)
		{
			ExceptionHandler resultingTypedNode;
			transformer.OnExceptionHandler((ExceptionHandler)this, out resultingTypedNode);
			resultingNode = resultingTypedNode;
		}
	}
}
