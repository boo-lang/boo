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

def Replace(folder as string, glob as string, expression as Regex, replacement as string):
	
	for fname in Directory.GetFiles(folder, glob):
		contents = ReadFile(fname)
		newContents = expression.Replace(contents, replacement)
		if newContents != contents:
			print(fname)
			WriteFile(fname, newContents)
			
	for subFolder in Directory.GetDirectories(folder):
		Replace(subFolder, glob, expression, replacement)

glob, expression, replacement = argv

Replace(".", glob, Regex(expression), replacement)

		
		
