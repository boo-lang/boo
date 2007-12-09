using System;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;

namespace BooCompiler.Tests
{
	public class AllParametersRequiredAttribute : AbstractAstAttribute
	{
		public AllParametersRequiredAttribute()
		{
		}

		override public void Apply(Node node)
		{
			Method m = node as Method;
			if (null == m)
			{
				throw new ApplicationException("This attribute can only be applied to methods.");
			}

			foreach (ParameterDeclaration pd in m.Parameters)
			{
				pd.Attributes.Add(new Boo.Lang.Compiler.Ast.Attribute("required"));
			}
		}
	}
}