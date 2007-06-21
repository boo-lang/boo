using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Boo.Lang.Compiler.Ast;
using Module=Boo.Lang.Compiler.Ast.Module;

namespace Boo.Lang.Compiler.MetaProgramming
{
	public class CompilationErrorsException : System.Exception
	{
		private CompilerErrorCollection _errors;

		public CompilationErrorsException(CompilerErrorCollection errors) : base(errors.ToString())
		{
			_errors = errors;
		}

		public CompilerErrorCollection Errors
		{
			get { return _errors;  }
		}
	}

	[CompilerGlobalScope]
	public sealed class Compilation
	{
		public static Type compile(ClassDefinition klass, params System.Reflection.Assembly[] references)
		{
			BooCompiler compiler = CreateLibraryCompiler(references);
			CompilerContext result = compiler.Run(CreateCompileUnit(klass));
			if (result.Errors.Count > 0) throw new CompilationErrorsException(result.Errors);
			return result.GeneratedAssembly.GetType(klass.Name);
		}

		private static BooCompiler CreateLibraryCompiler(Assembly[] references)
		{
			BooCompiler compiler = new BooCompiler();
			compiler.Parameters.OutputType = CompilerOutputType.Library;
			compiler.Parameters.Pipeline = new Boo.Lang.Compiler.Pipelines.CompileToMemory();
			compiler.Parameters.References.Extend(references);
			return compiler;
		}

		private static CompileUnit CreateCompileUnit(ClassDefinition klass)
		{
			return new CompileUnit(CreateModule(klass));
		}

		private static Module CreateModule(ClassDefinition klass)
		{
			Module module = new Module();
			module.Name = klass.Name;
			module.Members.Add(klass);
			return module;
		}
	}
}
