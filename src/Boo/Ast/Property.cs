using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class Property : PropertyImpl
	{		
		public Property()
		{
 		}
		
		public Property(Method getter, Method setter, TypeReference type) : base(getter, setter, type)
		{
		}
		
		public Property(antlr.Token token, Method getter, Method setter, TypeReference type) : base(token, getter, setter, type)
		{
		}
		
		internal Property(antlr.Token token) : base(token)
		{
		}
		
		internal Property(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnProperty(this);
		}
	}
}
