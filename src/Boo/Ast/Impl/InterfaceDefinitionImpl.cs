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
	}
}
