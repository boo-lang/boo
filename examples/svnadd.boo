import System.IO
import System.Text.RegularExpressions

def svnlocals(filter as Regex):
	localFiles = []
	for line as string in StringReader(shell("svn", "status")):
		if line.StartsWith("?") and (filter is null or filter.IsMatch(line)):
			localFiles.Add(/\s+/.Split(line)[-1]) 
	return localFiles 

filter = Regex(argv[-1]) if len(argv) > 0
locals = svnlocals(filter)
print(shell("svn", "add ${join(locals)}"))
