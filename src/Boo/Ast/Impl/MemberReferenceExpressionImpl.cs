using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class MemberReferenceExpressionImpl : ReferenceExpression
	{
		protected Expression _target;
		
		protected MemberReferenceExpressionImpl()
		{
 		}
		
		protected MemberReferenceExpressionImpl(Expression target)
		{
 			Target = target;
		}
		
		protected MemberReferenceExpressionImpl(antlr.Token token, Expression target) : base(token)
		{
 			Target = target;
		}
		
		internal MemberReferenceExpressionImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal MemberReferenceExpressionImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
 		}
		public Expression Target
		{
			get
			{
				return _target;
			}
			
			set
			{
				_target = value;
				if (null != _target)
				{
					_target.InitializeParent(this);
				}
			}
		}
	}
}
