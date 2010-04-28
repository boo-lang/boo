using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.Attributes
{
	/// <summary>
	/// Provides a simple way of enabling strict mode directly in code.
	/// 
	/// <example>[assembly: Boo.Lang.Compiler.Attributes.StrictMode]</example>
	/// </summary>
	public class StrictModeAttribute : AbstractAstAttribute
	{
		public override void Apply(Node targetNode)
		{
			Parameters.Strict = true;
		}
	}
}
