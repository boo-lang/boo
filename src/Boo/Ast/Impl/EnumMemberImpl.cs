using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class EnumMemberImpl : TypeMember
	{
		protected IntegerLiteralExpression _initializer;
		
		protected EnumMemberImpl()
		{
 		}
		
		protected EnumMemberImpl(IntegerLiteralExpression initializer)
		{
 			Initializer = initializer;
		}
		
		protected EnumMemberImpl(antlr.Token token, IntegerLiteralExpression initializer) : base(token)
		{
 			Initializer = initializer;
		}
		
		internal EnumMemberImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal EnumMemberImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
 		}
		public IntegerLiteralExpression Initializer
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
