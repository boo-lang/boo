using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class StatementModifierImpl : Node
	{
		protected StatementModifierType _type;
		protected Expression _condition;
		
		protected StatementModifierImpl()
		{
 		}
		
		protected StatementModifierImpl(StatementModifierType type, Expression condition)
		{
 			Type = type;
			Condition = condition;
		}
		
		protected StatementModifierImpl(antlr.Token token, StatementModifierType type, Expression condition) : base(token)
		{
 			Type = type;
			Condition = condition;
		}
		
		internal StatementModifierImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal StatementModifierImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
 		}
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.StatementModifier;
			}
		}
		public StatementModifierType Type
		{
			get
			{
				return _type;
			}
			
			set
			{
				_type = value;
			}
		}
		public Expression Condition
		{
			get
			{
				return _condition;
			}
			
			set
			{
				_condition = value;
				if (null != _condition)
				{
					_condition.InitializeParent(this);
				}
			}
		}
		public override void Switch(IAstTransformer transformer, out Node resultingNode)
		{
			StatementModifier resultingTypedNode;
			transformer.OnStatementModifier((StatementModifier)this, out resultingTypedNode);
			resultingNode = resultingTypedNode;
		}
	}
}
