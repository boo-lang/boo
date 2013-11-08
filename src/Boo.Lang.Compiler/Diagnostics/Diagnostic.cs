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
	}

	public class Range
	{
		/// TODO
	}

	public class Hint
	{
		/// TODO
	}
}

