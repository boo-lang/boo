"""
Recursive grep tool.

rgrep <GLOB> <PATTERN>
example: grep *.cs Boo.IO
"""
using System // Environment
using System.IO // Directory
using Boo.IO // TextFile

def ScanFile(fname as string, pattern as string):	
	for index as int, line as string in enumerate(TextFile(fname)):
		print("${fname}(${index}): ${line.Trim()}") if line =~ pattern
		
def ScanDirectory(path as string, glob as string, pattern as string):
	for fname in Directory.GetFiles(path, glob):
		ScanFile(fname, pattern)
	for path in Directory.GetDirectories(path):
		ScanDirectory(path, glob, pattern)

_, glob, pattern = Environment.GetCommandLineArgs()
ScanDirectory(".", glob, pattern)

