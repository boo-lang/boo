using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[System.Xml.Serialization.XmlInclude(typeof(Constructor))]
	[Serializable]
	public class Method : MethodImpl
	{		
		public Method()
		{
			_parameters = new ParameterDeclarationCollection(this);
			_returnTypeAttributes = new AttributeCollection(this);
			Body = new Block();
 		}
		
		public Method(TypeReference returnType) : base(returnType)
		{
		}
		
		public Method(antlr.Token token, TypeReference returnType) : base(token, returnType)
		{
		}
		
		internal Method(antlr.Token token) : base(token)
		{
		}
		
		internal Method(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnMethod(this);
		}
	}
}
