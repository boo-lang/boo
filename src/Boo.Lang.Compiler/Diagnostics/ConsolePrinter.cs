using System;
using System.IO;
using System.Text.RegularExpressions;


namespace Boo.Lang.Compiler.Diagnostics
{
	class ColorDiagnosticFormatProvider : DiagnosticFormatProvider
	{
		override protected string FormatterType(string[] opts, object arg)
		{
			return "{cyan}<" + arg + ">{reset}";
		}

		override protected string FormatterId(string[] opts, object arg)
		{
			return "'{cyan}" + arg + "{reset}'";
		}
	}


	public class ConsolePrinter
	{
		public bool ShowHints { get; set; }
		public int TabSize { get; set; }
		public int Indent { get; set; }
		public bool ShowCode { get; set; }
		public TextWriter Writer { get; set; }
		public IFormatProvider FormatProvider { get; set; }

		public ConsolePrinter()
		{
			ShowHints = true;
			TabSize = 4;
			ShowCode = false;
			Indent = 2;
			Writer = Console.Error;
			FormatProvider = new ColorDiagnosticFormatProvider();
		}

		protected void Write(string str, params object[] args)
		{
			str = String.Format(FormatProvider, str, args);

			var parts = Regex.Split(str, "{(\\w+)}");
			for (int i = 0; i < parts.Length; i++)
			{
				// Colors are always in odd positions 
				if (i % 2 == 1)
				{
					if (String.Equals("reset", parts[i], StringComparison.OrdinalIgnoreCase))
						Console.ResetColor();
					else
						Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), parts[i], true);
					continue;
				}

				Writer.Write(parts[i]);
			}

			Console.ResetColor();
		}

		protected void WriteLine(string str, params object[] args)
		{
			Write(str, args);
			Writer.WriteLine("");
		}

		public void Print(Diagnostic diag)
		{
			Print(diag, diag.Level);
		}

		public void Print(Diagnostic diag, DiagnosticLevel level)
		{
			Write("{0}({1},{2}): ", diag.Caret.FileName, diag.Caret.Line, diag.Caret.Column);

			var code = ": ";
			if (ShowCode)
				code = String.Format(":{0:0000}: ", diag.Code);

			switch (level) {
			case DiagnosticLevel.Note:
				Write("{{Gray}}note{0}", code);
				break;
			case DiagnosticLevel.Warning:
				Write("{{Magenta}}warning{0}", code);
				break;
			case DiagnosticLevel.Error:
				Write("{{Red}}error{0}", code);
				break;
			case DiagnosticLevel.Fatal:
				Write("{{DarkRed}}fatal{0}", code);
				break;
			}

			if (null != diag.Arguments)
				WriteLine(diag.Message, diag.Arguments);
			else
				WriteLine(diag.Message);

			if (!ShowHints)
				return;

			string line;
			try
			{
				var lines = File.ReadAllLines(diag.Caret.FullPath);
				line = lines[diag.Caret.Line - 1];
			} 
			catch (Exception x) 
			{
				WriteLine("{{magenta}}-- Unable to read source file --");
				return;
			}

			line = line.TrimEnd().Replace("\t", new String(' ', TabSize));
			int origLength = line.Length;
			line = line.TrimStart();
			int offset = origLength - line.Length;

			string indent = new String(' ', Indent);

			Write(indent);
			WriteLine("{{Gray}}" + line);

			if (null != diag.Hints)
			{
				// TODO: Merge multiple hints
				foreach (var hint in diag.Hints)
				{
					Write(indent);
					for (int i = 1; i < hint.Caret.Column - offset; i++)
						Write(" ");

					for (int i = 0; i < hint.Remove; i++)
						Write("{{Red}}~");

					if (!String.IsNullOrEmpty(hint.Insert))
					{
						WriteLine("{{Green}}^");

						Write(indent);
						for (int i = 1; i < hint.Caret.Column - offset; i++)
							Write(" ");

						Write("{{Yellow}}{0}", hint.Insert);
					}
					WriteLine("");
				}
			}
			else
			{
				Write(indent);
				for (int i = 1; i <= line.Length; i++) {
					if (i == diag.Caret.Column - offset) {
						Write("{{Blue}}^");
						continue;
					}

					bool done = false;
					if (null != diag.Ranges)
					{
						foreach (Range r in diag.Ranges)
						{
							if (i >= r.From.Column - offset && i < r.Until.Column - offset) {
								Write("{{Blue}}~");
								done = true;
								break;
							}
						}
					}
					if (!done)
						Write(" ");
				}
				WriteLine("");
			}
		}
	}

}

