using System;

namespace Boo.Ast.Compilation
{
	/// <summary>
	/// An input to the compiler.
	/// </summary>
	public interface ICompilerInput
	{
		string Name
		{
			get;
		}

		System.IO.TextReader Open();
	}
}
