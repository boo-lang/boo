using System;
using System.IO;
using Boo.Ast;
using Boo.Ast.Compilation;

namespace Boo.Ast.Compilation
{
	/// <summary>
	/// The compiler: a façade to the CompilerParameters/CompilerContext/Pipeline subsystem.
	/// </summary>
	public class Compiler
	{
		CompilerParameters _parameters;

		public Compiler()
		{
			_parameters = new CompilerParameters();
		}

		public CompilerParameters Parameters
		{
			get
			{
				return _parameters;
			}
		}

		public CompilerContext Run()
		{
			CompilerContext context = new CompilerContext(_parameters, new CompileUnit());
			context.Run();
			return context;
		}		
	}
}
