"""
grep <GLOB> <PATTERN>
example: grep *.cs Boo.IO
"""
using System // Environment
using System.IO // Directory
using Boo.IO // TextFile

def ScanFile(fname as string, pattern as string):	
	for index, line in enumerate(TextFile(fname)):
		print("${fname}(${index}): ${line}") if line =~ pattern

_, glob, pattern = Environment.GetCommandLineArgs()
for fname in Directory.GetFiles(".", glob):
	ScanFile(fname, pattern)

