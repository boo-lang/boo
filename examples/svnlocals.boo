import System.Text.RegularExpressions

localFiles = []
for line in shell("svn status"):
	localFiles.Add(/\s+/.Split(line)[-1]) if line.StartsWith("?")
	
filter = Regex(argv[-1]) if len(argv) > 1
if filter:
	for fname in localFiles:
		print(fname) if filter =~ fname
else:
	print(join(localFiles, "\n"))
