import System.IO

for line as string in StringReader(shell("svn", "status")):
	print(/\s+/.Split(line)[-1]) if line =~ /^(A|M|D)/

