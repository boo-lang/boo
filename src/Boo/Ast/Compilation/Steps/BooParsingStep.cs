using System;
using Boo.Ast.Parsing;
using Boo.Ast.Compilation;

namespace Boo.Ast.Compilation.Steps
{
	/// <summary>
	/// Step 1. Parses any input fed to the compiler.
	/// </summary>
	public class BooParsingStep : ICompilerStep
	{
		CompilerContext _context;

		public BooParsingStep()
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
			try
			{
				ParserErrorHandler errorHandler = new ParserErrorHandler(OnParserError);

				foreach (ICompilerInput input in _context.CompilerParameters.Input)
				{
					try
					{
						using (System.IO.TextReader reader = input.Open())
						{
							Module module = BooParser.ParseModule(input.Name, reader, errorHandler);
							if (null != module)
							{
								_context.CompileUnit.Modules.Add(module);
							}
						}
					}
					catch (Exception x)
					{
						_context.Errors.InputError(input, x);
					}
				}
			}
			finally
			{
				_context = null;
			}
		}

		void OnParserError(antlr.RecognitionException error)
		{
			_context.Errors.ParserError(error);
		}
	}
}
