using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;

// ReSharper disable CheckNamespace
namespace Boo.Lang
// ReSharper restore CheckNamespace
{
	/// <summary>
	/// Provides a simple way of enabling strict mode directly in code.
	/// 
	/// <example>[assembly: StrictMode]</example>
	/// </summary>
	public class StrictModeAttribute : AbstractAstAttribute
	{
		public override void Apply(Node targetNode)
		{
			if (!(targetNode is CompileUnit))
				Context.Warnings.Add(CompilerWarningFactory.CustomWarning(LexicalInfo, "Use [assembly: StrictMode]"));
			Parameters.Strict = true;
		}
	}
}
