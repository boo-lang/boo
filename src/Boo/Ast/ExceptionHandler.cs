using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class ExceptionHandler : ExceptionHandlerImpl
	{		
		public ExceptionHandler()
		{
 		}
		
		public ExceptionHandler(Declaration declaration) : base(declaration)
		{
		}
		
		public ExceptionHandler(antlr.Token token, Declaration declaration) : base(token, declaration)
		{
		}
		
		internal ExceptionHandler(antlr.Token token) : base(token)
		{
		}
		
		internal ExceptionHandler(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnExceptionHandler(this);
		}
	}
}
