using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class AsExpressionImpl : Expression
	{
		protected Expression _target;
		protected TypeReference _type;
		
		protected AsExpressionImpl()
		{
 		}
		
		protected AsExpressionImpl(Expression target, TypeReference type)
		{
 			Target = target;
			Type = type;
		}
		
		protected AsExpressionImpl(antlr.Token token, Expression target, TypeReference type) : base(token)
		{
 			Target = target;
			Type = type;
		}
		
		internal AsExpressionImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal AsExpressionImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
 		}
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.AsExpression;
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
				_target = value;
				if (null != _target)
				{
					_target.InitializeParent(this);
				}
			}
		}
		public TypeReference Type
		{
			get
			{
				return _type;
			}
			
			set
			{
				_type = value;
				if (null != _type)
				{
					_type.InitializeParent(this);
				}
			}
		}
	}
}
