using System;

namespace Boo.Ast.Compilation
{
	/// <summary>
	/// A step in the compilation pipeline.
	/// </summary>
	public interface ICompilerStep : IDisposable
	{
		void Initialize(CompilerContext context);
		
		void Run();
	}
}
