"""
grep <GLOB> <PATTERN>
example: grep *.cs Boo.IO
"""
import System.IO // Directory

def ScanFile(fname as string, pattern as string):	
	for index, line in enumerate(File.OpenText(fname)):
		print("${fname}(${index}): ${line}") if line =~ pattern

glob, pattern = argv
for fname in Directory.GetFiles(".", glob):
	ScanFile(fname, pattern)

