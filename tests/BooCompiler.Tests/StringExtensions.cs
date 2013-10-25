using System;
using System.Linq;

namespace BooCompiler.Tests
{
	public static class StringExtensions
	{
		public static string ReIndent(this string code)
		{
			var lines = code.Split('\n').ToList();
			var firstNonBlankLine = lines.FirstOrDefault(line => line.Trim().Length > 0);
			if (firstNonBlankLine != null)
			{
				var indentation = new string(firstNonBlankLine.TakeWhile(Char.IsWhiteSpace).ToArray());
				return string.Join("\n",
					lines.Select(line => line.StartsWith(indentation) ? line.Substring(indentation.Length) : line).ToArray());
			}
			return code;
		}
	}
}