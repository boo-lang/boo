using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class ConstructorImpl : Method
	{
		
		protected ConstructorImpl()
		{
 		}
		
		internal ConstructorImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal ConstructorImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
 		}
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.Constructor;
			}
		}
		public override void Switch(IAstTransformer transformer, out Node resultingNode)
		{
			Constructor resultingTypedNode;
			transformer.OnConstructor((Constructor)this, out resultingTypedNode);
			resultingNode = resultingTypedNode;
		}
	}
}
