using System;
using Boo.Lang.Compiler;

namespace Boo.Lang.Compiler.Diagnostics
{
	// Delegate for the notification of new diagnostics
	public delegate void DiagnosticEventHandler(DiagnosticLevel level, Diagnostic diag);
	public delegate void ContextEventHandler(CompilerContext context);
	public delegate void InputEventHandler(ICompilerInput input);


	/// <summary>
	/// Serves as main interface between the compiler and the diagnostics sub system.
	/// </summary>
	public class DiagnosticsEngine
	{
		/// <summary>
		/// Notify successfully consumed diagnostics
		/// </summary>
		public event DiagnosticEventHandler Handler;

		/// <summary>
		/// Notify a new diagnostics session
		/// </summary>
		public event ContextEventHandler OnStartContext;

		/// <summary>
		/// Notify a new file being analyzed
		/// </summary>
		public event InputEventHandler OnStartFile;


		public bool IgnoreAllWarnings {	get; set; }
		public bool WarningsAsErrors { get; set; }
		public bool ErrorsAsFatal { get; set; }   
		public bool SuppressAllDiagnostics { get; set; }
		public int ErrorLimit { get; set; }
		public int[] IgnoredCodes { get; set; }
		public int[] PromotedCodes { get; set; }

		private bool fatalOcurred;
		public bool FatalOcurred { get { return fatalOcurred; } }
		private int noteCount;
		public int NoteCount { get { return noteCount; } }
		private int warningCount;
		public int WarningCount { get { return warningCount; } }
		private int errorCount;
		public int ErrorCount {
			get { return errorCount + (fatalOcurred ? 1 : 0); }
		}

		public int Count {
			get { return ErrorCount + WarningCount + NoteCount; }
		}

		public bool HasErrors {
			get { return ErrorCount > 0; }
		}

		public void StartContext(CompilerContext context)
		{
			if (null != OnStartContext)
				OnStartContext(context);
		}

		public void StartFile(ICompilerInput inp)
		{
			if (null != OnStartFile)
				OnStartFile(inp);
		}

		/// <summary>
		/// Reset error counters
		/// </summary>
		virtual public void Reset()
		{
			noteCount = 0;
			warningCount = 0;
			errorCount = 0;
			fatalOcurred = false;
		}

		/// <summary>
		/// Maps a diagnostic to a normalized severity level based on the configuration
		/// </summary>
		virtual protected DiagnosticLevel Map(Diagnostic diag)
		{
			if (FatalOcurred || SuppressAllDiagnostics)
				return DiagnosticLevel.Ignored;

			var level = diag.Level;

			if (null != IgnoredCodes && -1 != Array.IndexOf(IgnoredCodes, diag.Code))
				level = DiagnosticLevel.Ignored;

			if (null != PromotedCodes && -1 != Array.IndexOf(PromotedCodes, diag.Code)) 
			{
				switch (level) {
				case DiagnosticLevel.Ignored:
					level = DiagnosticLevel.Note;
					break;
				case DiagnosticLevel.Note:
					level = DiagnosticLevel.Warning;
					break;
				case DiagnosticLevel.Warning:
					level = DiagnosticLevel.Error;
					break;
				case DiagnosticLevel.Error:
					level = DiagnosticLevel.Fatal;
					break;
				}
			}

			if (WarningsAsErrors && level == DiagnosticLevel.Warning)
				level = DiagnosticLevel.Error;

			if (IgnoreAllWarnings && level == DiagnosticLevel.Warning)
				level = DiagnosticLevel.Ignored;

			if (ErrorsAsFatal && level == DiagnosticLevel.Error)
				level = DiagnosticLevel.Fatal;

			if (ErrorLimit != 0 && ErrorCount >= ErrorLimit)
				level = DiagnosticLevel.Ignored;

			return level;
		}


		/// <summary>
		/// Consume a diagnostic produced by the compiler to notify the configured
		/// handlers if needed.
		/// </summary>
		virtual public void Consume(Diagnostic diag)
		{
			var level = Map(diag);

			switch (level) {
			case DiagnosticLevel.Ignored:
				return;
			case DiagnosticLevel.Fatal:
				fatalOcurred = true;
				break;
			case DiagnosticLevel.Error:
				errorCount += 1;
				break;
			case DiagnosticLevel.Warning:
				warningCount += 1;
				break;
			case DiagnosticLevel.Note:
				noteCount += 1;
				break;
			}

			if (null != Handler)
				Handler(level, diag);
		}
	}
}