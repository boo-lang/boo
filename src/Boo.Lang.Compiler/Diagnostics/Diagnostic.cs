using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.Diagnostics
{
	/// <summary>
	/// Represents a diagnostic. It can either be subclassed for common cases
	/// or generated via factories to reduce code duplication.
	/// </summary>
	public class Diagnostic
	{
		/// <summary>
		/// Severity of the diagnostic
		/// </summary>
		public DiagnosticLevel Level { get; set; }

		/// <summary>
		/// Unique code of the diagnostic
		/// </summary>
		public int Code { get; set; }

		/// <summary>
		/// Reported message. It can use placeholders for Arguments
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// Values to fill the placeholders in the message
		/// </summary>
		public object[] Arguments { get; set; }

		/// <summary>
		/// Exact position where the diagnostic found the problem
		/// </summary>
		public LexicalInfo Caret { get; set; }

		/// <summary>
		/// Signals important aspects of the original code
		/// </summary>
		/// <value>The ranges.</value>
		public Range[] Ranges { get; set; }

		/// <summary>
		/// Additional information about how to solve this specific problem
		/// </summary>
		public Hint[] Hints { get; set; }

		static public Diagnostic FromCompilerError(CompilerError error)
		{
			var diag = new Diagnostic();
			int code;
			int.TryParse(error.Code.Substring(3), out code);
			diag.Code = code;
			diag.Level = DiagnosticLevel.Error;
			diag.Message = error.Message;
			diag.Caret = error.LexicalInfo;
			return diag;
		}

		static public Diagnostic FromCompilerWarning(CompilerWarning warning)
		{
			var diag = new Diagnostic();
			int code;
			int.TryParse(warning.Code.Substring(3), out code);
			diag.Code = code;
			diag.Level = DiagnosticLevel.Warning;
			diag.Message = warning.Message;
			diag.Caret = warning.LexicalInfo;
			return diag;
		}		
	}

	/// <summary>
	/// Represents a segment of the source code
	/// </summary>
	public class Range
	{
		public SourceLocation From { get; set; }
		public SourceLocation Until { get; set; }

		public Range(SourceLocation from, SourceLocation until)
		{
			From = from;
			Until = until;
		}

		public Range(SourceLocation from, int length)
		{
			From = from;
			Until = new SourceLocation(from.Line, from.Column + length);
		}

		public Range(Node node)
		{
			From = node.LexicalInfo;
			Until = new SourceLocation(From.Line, From.Column + node.ToCodeString().Length);
		}
	}

	/// <summary>
	/// Represents a code modification:
    ///  - Insert: Remove = 0, Insert = 'foo'
    ///  - Replace: Remove = 5, Insert = 'foo'
    ///  - Remove: Remove = 5, Insert = ''
	/// </summary>
	public class Hint
	{
	    public SourceLocation Caret { get; set; }
    	public int Remove { get; set; }
   	 	public string Insert { get; set; }

   	 	public Hint(SourceLocation caret, int remove, string insert)
   	 	{
   	 		Caret = caret;
   	 		Insert = insert;
   	 		Remove = remove;
   	 	}

   	 	public Hint(SourceLocation caret, int remove) : this(caret, remove, null)
   	 	{
   	 	}

   	 	public Hint(SourceLocation caret, string insert) : this(caret, 0, insert)
   	 	{
   	 	}
	}
}

