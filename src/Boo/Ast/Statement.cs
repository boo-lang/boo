using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[System.Xml.Serialization.XmlInclude(typeof(DeclarationStatement))]
	[System.Xml.Serialization.XmlInclude(typeof(AssertStatement))]
	[System.Xml.Serialization.XmlInclude(typeof(TryStatement))]
	[System.Xml.Serialization.XmlInclude(typeof(IfStatement))]
	[System.Xml.Serialization.XmlInclude(typeof(ForStatement))]
	[System.Xml.Serialization.XmlInclude(typeof(WhileStatement))]
	[System.Xml.Serialization.XmlInclude(typeof(GivenStatement))]
	[System.Xml.Serialization.XmlInclude(typeof(BreakStatement))]
	[System.Xml.Serialization.XmlInclude(typeof(ContinueStatement))]
	[System.Xml.Serialization.XmlInclude(typeof(RetryStatement))]
	[System.Xml.Serialization.XmlInclude(typeof(ReturnStatement))]
	[System.Xml.Serialization.XmlInclude(typeof(YieldStatement))]
	[System.Xml.Serialization.XmlInclude(typeof(RaiseStatement))]
	[System.Xml.Serialization.XmlInclude(typeof(UnpackStatement))]
	[System.Xml.Serialization.XmlInclude(typeof(ExpressionStatement))]
	[Serializable]
	public abstract class Statement : StatementImpl
	{		
		public Statement()
		{
 		}
		
		public Statement(StatementModifier modifier) : base(modifier)
		{
		}
		
		public Statement(antlr.Token token, StatementModifier modifier) : base(token, modifier)
		{
		}
		
		internal Statement(antlr.Token token) : base(token)
		{
		}
		
		internal Statement(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
	}
}
