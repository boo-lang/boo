using System;

namespace Boo.Ast.Compilation
{
	/// <summary>
	/// AST attributes to customize the code generation
	/// for language elements.
	/// </summary>
	public class CodeGeneratorAttributes
	{
		/// <summary>
		/// Marks the method as the entry point for the application.
		/// Boolean.
		/// </summary>
		public const string EntryPoint = "entrypoint";
	}
}
