import System // Environment
import System.IO // Directory
import Boo.IO // TextFile

def ScanFile(fname as string, expression as string):	
	for index, line as string in enumerate(TextFile(fname)):
		print("${fname}(${index}): ${line}") if line =~ expression

_, glob, expression = Environment.GetCommandLineArgs()
for fname in Directory.GetFiles(".", glob):
	ScanFile(fname, expression)

