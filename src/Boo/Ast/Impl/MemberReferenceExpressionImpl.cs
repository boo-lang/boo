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
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.MemberReferenceExpression;
			}
		}
		public Expression Target
		{
			get
			{
				return _target;
			}
			
			set
			{
				
				if (_target != value)
				{
					_target = value;
					if (null != _target)
					{
						_target.InitializeParent(this);
					}
				}
			}
		}
		public override void Switch(IAstTransformer transformer, out Node resultingNode)
		{
			MemberReferenceExpression thisNode = (MemberReferenceExpression)this;
			Expression resultingTypedNode = thisNode;
			transformer.OnMemberReferenceExpression(thisNode, ref resultingTypedNode);
			resultingNode = resultingTypedNode;
		}
	}
}
