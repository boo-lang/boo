using System;
using Boo.Ast.Compilation;

namespace Boo.Ast.Compilation.Steps
{
	/// <summary>	
	/// </summary>
	public class BooPrinterStep : ICompilerStep
	{
		CompilerContext _context;
		
		public BooPrinterStep()
		{
		}
		
		public void Initialize(CompilerContext context)
		{
			_context = context;
		}
		
		public void Dispose()
		{
			_context = null;
		}

		public void Run()
		{		
			Boo.Ast.Visiting.BooPrinterVisitor visitor = new Boo.Ast.Visiting.BooPrinterVisitor(Console.Out);
			visitor.Print(_context.CompileUnit);
		}
	}
}
