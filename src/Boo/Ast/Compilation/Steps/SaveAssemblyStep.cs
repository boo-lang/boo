using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using Boo.Ast;
using Boo.Ast.Compilation;

namespace Boo.Ast.Compilation.Steps
{
	public class SaveAssemblyStep : ICompilerStep
	{
		CompilerContext _context;
		
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
			if (_context.Errors.Count > 0)
			{
				return;
			}
			
			AssemblyBuilder builder = AssemblySetupStep.GetAssemblyBuilder(_context);
			builder.Save(AssemblySetupStep.GetOutputAssemblyFileName(_context));
		}	
	}
}
