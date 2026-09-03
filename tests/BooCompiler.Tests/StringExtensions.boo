namespace BooCompiler.Tests

import System
import System.Linq
import Boo.Lang

static class StringExtensions:
	[Extension]
	public def ReIndent(code as string) as string:
		lines = code.Split(char('\n'))
		firstNonBlankLine as string = null
		for line in lines:
			if line.Trim().Length > 0:
				firstNonBlankLine = line
				break
		return code if firstNonBlankLine is null

		indentation = string(firstNonBlankLine.TakeWhile(Char.IsWhiteSpace).ToArray())
		reindented = List[of string]()
		for line in lines:
			if line.StartsWith(indentation):
				reindented.Add(line.Substring(indentation.Length))
			else:
				reindented.Add(line)
		return string.Join("\n", reindented.ToArray())
