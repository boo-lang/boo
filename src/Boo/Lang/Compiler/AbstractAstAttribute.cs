namespace Boo.Lang.Compiler
{
	using System;
	
	public abstract class AbstractAstAttribute : AbstractCompilerComponent, IAstAttribute
	{
		protected Boo.Lang.Ast.Attribute _attribute;
		
		public Boo.Lang.Ast.Attribute Attribute
		{
			set
			{				
				_attribute = value;
			}
		}

		public Boo.Lang.Ast.LexicalInfo LexicalInfo
		{
			get
			{
				return _attribute.LexicalInfo;
			}
		}
		
		public abstract void Apply(Boo.Lang.Ast.Node targetNode);
	}
}
