using System;

namespace Boo.Ast.Compilation.IO
{
	/// <summary>
	/// String based compiler input.
	/// </summary>
	public class StringInput : ReaderInput
	{
		public StringInput(string name, string contents) : base(name, new System.IO.StringReader(contents))
		{
		}
	}
}
