using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class InterfaceDefinitionImpl : TypeDefinition
	{
		
		protected InterfaceDefinitionImpl()
		{
 		}
		
		internal InterfaceDefinitionImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal InterfaceDefinitionImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
 		}
		
		public override NodeType NodeType
		{
			get
			{
				return NodeType.InterfaceDefinition;
			}
		}
		public override void Switch(IAstTransformer transformer, out Node resultingNode)
		{
			InterfaceDefinition thisNode = (InterfaceDefinition)this;
			InterfaceDefinition resultingTypedNode = thisNode;
			transformer.OnInterfaceDefinition(thisNode, ref resultingTypedNode);
			resultingNode = resultingTypedNode;
		}
	}
}
