using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class SlicingExpressionImpl : Expression
	{
		protected Expression _target;
		protected Expression _begin;
		protected Expression _end;
		protected Expression _step;
		
		protected SlicingExpressionImpl()
		{
 		}
		
		protected SlicingExpressionImpl(Expression target, Expression begin, Expression end, Expression step)
		{
 			Target = target;
			Begin = begin;
			End = end;
			Step = step;
		}
		
		protected SlicingExpressionImpl(antlr.Token token, Expression target, Expression begin, Expression end, Expression step) : base(token)
		{
 			Target = target;
			Begin = begin;
			End = end;
			Step = step;
		}
		
		internal SlicingExpressionImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal SlicingExpressionImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
 		}
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.SlicingExpression;
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
		public Expression Begin
		{
			get
			{
				return _begin;
			}
			
			set
			{
				
				if (_begin != value)
				{
					_begin = value;
					if (null != _begin)
					{
						_begin.InitializeParent(this);
					}
				}
			}
		}
		public Expression End
		{
			get
			{
				return _end;
			}
			
			set
			{
				
				if (_end != value)
				{
					_end = value;
					if (null != _end)
					{
						_end.InitializeParent(this);
					}
				}
			}
		}
		public Expression Step
		{
			get
			{
				return _step;
			}
			
			set
			{
				
				if (_step != value)
				{
					_step = value;
					if (null != _step)
					{
						_step.InitializeParent(this);
					}
				}
			}
		}
		public override void Switch(IAstTransformer transformer, out Node resultingNode)
		{
			SlicingExpression thisNode = (SlicingExpression)this;
			Expression resultingTypedNode = thisNode;
			transformer.OnSlicingExpression(thisNode, ref resultingTypedNode);
			resultingNode = resultingTypedNode;
		}
	}
}
