using System;
using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.Steps
{
	public class ExpandAstLiterals : AbstractTransformerCompilerStep
	{	
		public ExpandAstLiterals()
		{
		}

        override public void Run()
        {
        	Visit(CompileUnit);
        }

        override public void OnAstLiteralExpression(AstLiteralExpression node)
        {
			Type type = node.Node.GetType();
			CastExpression ce = new CastExpression(CodeBuilder.CreateTypeReference(type), CreateFromXmlInvocation(node.Node.LexicalInfo, type, AstUtil.ToXml(node.Node)));
			ce.LexicalInfo = node.LexicalInfo;

			ReplaceCurrentNode(ce);
        }

		private Expression CreateFromXmlInvocation(LexicalInfo li, Type type, string xml)
		{
			MethodInvocationExpression e = new MethodInvocationExpression(li);
			e.Target = AstUtil.CreateReferenceExpression("Boo.Lang.Compiler.Ast.AstUtil.FromXml");
			e.Arguments.Add(CodeBuilder.CreateTypeofExpression(type));
			e.Arguments.Add(new StringLiteralExpression(xml));
			return e;
		}
	}
}
