using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class ClassDefinitionImpl : TypeDefinition
	{
		
		protected ClassDefinitionImpl()
		{
 		}
		
		internal ClassDefinitionImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal ClassDefinitionImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
 		}
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.ClassDefinition;
			}
		}
		public override void Switch(IAstTransformer transformer, out Node resultingNode)
		{
			ClassDefinition resultingTypedNode;
			transformer.OnClassDefinition((ClassDefinition)this, out resultingTypedNode);
			resultingNode = resultingTypedNode;
		}
	}
}
