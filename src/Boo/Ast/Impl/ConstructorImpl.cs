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
	}
}
