using System;

namespace Boo.Lang.Compiler.Diagnostics
{
	/// <summary>
	/// Custom formats for diagnostic messages.
	/// </summary>
	public class DiagnosticFormatProvider : IFormatProvider, ICustomFormatter
	{
		public object GetFormat(Type formatType) {
			if (formatType == typeof(ICustomFormatter))
				return this;
			else 
				return null;
		}

		public string Format(string format, object arg, IFormatProvider formatProvider) {
			if (String.IsNullOrEmpty(format))
				return arg.ToString();

			string[] parts = format.Split(new char[] { '=' }, 2);
			string formatter = parts[0];
			string[] opts;
			if (parts.Length > 1)
				opts = parts[1].Split('|');
			else
				opts = new string[0];

			return Apply(formatter, opts, arg);
		}

		virtual protected string Apply(string formatter, string[] opts, object arg)
		{
			switch (formatter)
			{
			case "type": 
				return FormatterType(opts, arg);
			case "id":
				return FormatterId(opts, arg);
			case "choice":
				return FormatterChoice(opts, arg);
			case "plural":
				return FormatterPlural(opts, arg);
			case "seq":
				return FormatterSeq(opts, arg);				
			case "or":
				return FormatterOr(opts, arg);
			case "and":
				return FormatterAnd(opts, arg);
			case "ord":
				return FormatterOrd(opts, arg);
			}

			// TODO: Trace some warning about this
			return arg.ToString();
		}

		virtual protected string FormatterType(string[] opts, object arg)
		{
			return "<" + arg + ">";
		}

		virtual protected string FormatterId(string[] opts, object arg)
		{
			return "'" + arg + "'";
		}

		protected int GetNumber(object arg)
		{
			int idx;
			if (arg is System.Collections.IList)
				idx = ((System.Collections.IList)arg).Count;
			else if (arg is Array)
				idx = ((Array)arg).Length;
			else if (arg is String)
				int.TryParse((String)arg, out idx);
			else if (arg is int)
				idx = (int)arg;
			else
				idx = -1;

			return idx;
		}

		virtual protected string FormatterChoice(string[] opts, object arg)
		{
			int idx = GetNumber(arg);
			if (-1 == idx)
			{
				return arg.ToString();  // TODO: Trace some warning
			}

			string last = "";
			for (int i=0; i<=idx && i<opts.Length; i++) {
				if (!String.IsNullOrEmpty(opts[i])) 
					last = opts[i];
			}

			return last;
		}

		virtual protected string FormatterPlural(string[] opts, object arg)
		{
			// Just insert the last opt as the first to to handle 0
			var lopts = new List<string>(opts);
			lopts.Insert(0, lopts[lopts.Count - 1]);
			return FormatterChoice(lopts.ToArray(), arg);
		}

		virtual protected string FormatterSeq(string[] opts, object arg)
		{
			System.Collections.IList args; 
			if (arg is System.Collections.IList)
				args = ((System.Collections.IList)arg);
			else if (arg is Array)
				args = new List((object[])arg);
			else
				return arg.ToString(); // TODO: Trace warning

			if (1 == args.Count)
				return args[0].ToString();

			if (opts.Length > 0) 
			{
				var last = args[args.Count - 1];
				args.RemoveAt(args.Count - 1);

				var seq = new string[args.Count];
				args.CopyTo(seq, 0);

				return String.Join(", ", seq) + " " + opts[0] + " " + last;
			}
			else
			{
				var seq = new string[args.Count];
				args.CopyTo(seq, 0);
				return String.Join(", ", seq);
			}
		}

		virtual protected string FormatterOr(string[] opts, object arg)
		{
			return FormatterSeq(new string[] { "or" }, arg);
		}

		virtual protected string FormatterAnd(string[] opts, object arg)
		{
			return FormatterSeq(new string[] { "and" }, arg);
		}

		virtual protected string FormatterOrd(string[] opts, object arg)
		{
			int num = GetNumber(arg);
			if (-1 == num)
			{
				return arg.ToString();  // TODO: Trace some warning
			}

			if (num < 1)
				return num.ToString();  // No ordinal representation for 0

			switch(num % 100) {
				case 11:
				case 12:
				case 13:
				return num + "th";
			}

			switch(num % 10) {
				case 1:
				return num + "st";
				case 2:
				return num + "nd";
				case 3:
				return num + "rd";
				default:
				return num + "th";
			}
		}
	}
}

