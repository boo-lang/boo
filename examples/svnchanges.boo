import System.IO

for line in StringReader(shell("svn", "status")):
	print(/\s+/.Split(line)[-1].Replace("\\", "/")) if line =~ /^(A|M|D)/

