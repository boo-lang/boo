using System;

namespace Boo.Ast.Impl
{
	[Serializable]
	public abstract class EnumDefinitionImpl : TypeDefinition
	{
		
		protected EnumDefinitionImpl()
		{
 		}
		
		internal EnumDefinitionImpl(antlr.Token token) : base(token)
		{
 		}
		
		internal EnumDefinitionImpl(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
 		}
	}
}
