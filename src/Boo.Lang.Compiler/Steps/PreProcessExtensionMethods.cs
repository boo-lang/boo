using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.Steps
{
	/// <summary>
	/// </summary>
	public class PreProcessExtensionMethods : AbstractTransformerCompilerStep
	{
		public PreProcessExtensionMethods()
		{
		}

		public override void Run()
		{
			Visit(CompileUnit);
		}

		public override void OnConstructor(Constructor node)
		{
		}

		public override void OnDestructor(Destructor node)
		{
		}

		public override void OnProperty(Property node)
		{	
		}

		public override void OnField(Field node)
		{	
		}

		public override void OnMethod(Boo.Lang.Compiler.Ast.Method node)
		{
			if (MethodImplementationFlags.Extension != (node.ImplementationFlags & MethodImplementationFlags.Extension)) return;

			Visit(node.Body);
		}

		public override void OnSelfLiteralExpression(SelfLiteralExpression node)
		{
			ReplaceCurrentNode(new ReferenceExpression(node.LexicalInfo, "self"));
		}
	}
}
