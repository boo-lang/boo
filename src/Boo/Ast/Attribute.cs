using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class Attribute : AttributeImpl
	{		
		public Attribute()
		{
			_arguments = new ExpressionCollection(this);
			_namedArguments = new ExpressionPairCollection(this);
 		}
		
		public Attribute(string name) : base(name)
		{
		}
		
		public Attribute(antlr.Token token, string name) : base(token, name)
		{
		}
		
		internal Attribute(antlr.Token token) : base(token)
		{
		}
		
		internal Attribute(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnAttribute(this);
		}
	}
}
