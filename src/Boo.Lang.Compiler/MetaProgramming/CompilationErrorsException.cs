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
}