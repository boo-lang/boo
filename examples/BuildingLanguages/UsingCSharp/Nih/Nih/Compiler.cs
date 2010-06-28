using System;
using System.Reflection;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.IO;
using Boo.Lang.Compiler.MetaProgramming;

namespace Nih
{
	public class Compiler
	{
		public static Assembly CompileString(string code)
		{
			var compiler = new BooCompiler(new CompilerParameters(false));
			
			compiler.Parameters.GenerateInMemory = true;
			compiler.Parameters.Pipeline = new NihPipeline();
			compiler.Parameters.Input.Add(new StringInput("string.nih", code));

			var result = compiler.Run();
			if (result.Errors.Count > 0)
				throw new CompilationErrorsException(result.Errors);
			
			return result.GeneratedAssembly;
		}
	}
}
