using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[System.Xml.Serialization.XmlInclude(typeof(ExceptionHandler))]
	[System.Xml.Serialization.XmlInclude(typeof(WhenClause))]
	[Serializable]
	public class Block : BlockImpl, IMultiLineStatement
	{		
		public Block()
		{
			_statements = new StatementCollection(this);
 		}
		
		internal Block(antlr.Token token) : base(token)
		{
		}
		
		internal Block(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnBlock(this);
		}
	}
}
