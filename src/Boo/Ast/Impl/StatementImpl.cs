using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class StatementImpl : Node
	{
		protected StatementModifier _modifier;
		
		protected StatementImpl()
		{
 		}
		
		protected StatementImpl(StatementModifier modifier)
		{
 			Modifier = modifier;
		}
		
		protected StatementImpl(antlr.Token token, StatementModifier modifier) : base(token)
		{
 			Modifier = modifier;
		}
		
		internal StatementImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal StatementImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
 		}
		public StatementModifier Modifier
		{
			get
			{
				return _modifier;
			}
			
			set
			{
				_modifier = value;
				if (null != _modifier)
				{
					_modifier.InitializeParent(this);
				}
			}
		}
	}
}
