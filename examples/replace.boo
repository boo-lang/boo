"""
Regular expression replacement tool - replaces every
occurrence of a regular expression in a group of files.

Usage:
	replace.boo <FILE GLOB> <EXPRESSION> <REPLACEMENT>
	
Examples:
	replace.boo *.cs "AssemblyVersion(.*?)" "AssemblyVersion(1.2.3.4)"
"""
import System
import System.Text.RegularExpressions
import System.IO
import Boo.IO.TextFile // static members of TextFile will be available in the global scope

glob, expression, replacement = argv

re = Regex(expression)
for fname in Directory.GetFiles(".", glob):
	contents = ReadFile(fname)
	newContents = re.Replace(contents, replacement)
	if newContents != contents:
		print(fname)
		WriteFile(fname, newContents)
		
		
