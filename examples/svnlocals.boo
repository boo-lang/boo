import System.IO
import System.Text.RegularExpressions

def svnlocals():
	localFiles = []
	for line as string in StringReader(shell("svn", "status")):
		localFiles.Add(/\s+/.Split(line)[-1]) if line.StartsWith("?")
	return localFiles 

filter = Regex(argv[-1]) if len(argv) > 1
if filter:
	for fname as string in svnlocals():
		print(fname) if fname =~ filter
else:
	print(join(svnlocals(), "\n"))
