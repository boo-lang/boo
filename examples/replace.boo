"""
Regular expression replacement tool - replaces every
occurrence of a regular expression in a group of files.

Usage:
	replace.boo <FILE GLOB> <EXPRESSION> <REPLACEMENT>
	
Examples:
	replace.boo *.cs "AssemblyVersion(.*?)" "AssemblyVersion(1.2.3.4)"
"""
using System
using System.Text.RegularExpressions
using System.IO
using Boo.IO.TextFile

_, glob, expression, replacement = Environment.GetCommandLineArgs()

re = Regex(expression)
for fname in Directory.GetFiles(".", glob):
	contents = ReadFile(fname)
	newContents = re.Replace(contents, replacement)
	if newContents != contents:
		print(fname)
		WriteFile(fname, contents)
		
		
