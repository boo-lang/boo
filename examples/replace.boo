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

def read(fname as string):
	using stream=File.OpenText(fname):
		return stream.ReadToEnd()
		
def write(fname as string, contents as string):
	using stream=File.OpenWrite(fname):
		stream.Write(contents)

_, glob, expression, replacement = Environment.GetCommandLineArgs()

re = Regex(expression)
for fname in Directory.GetFiles(".", glob):
	contents = read(fname)
	newContents = re.Replace(contents, replacement)
	write(fname, contents)
		
		
