import System

def svn(cmd as string):
	print("svn ${cmd}")
	p = shellp("svn", cmd)
	response = p.StandardOutput.ReadToEnd()	
	p.WaitForExit()
	raise p.StandardError.ReadToEnd() if p.ExitCode
	return response

trunk = Uri(/URL:\s(.+)/.Match(svn("info")).Groups[1].Value)
stable = Uri(trunk, "branches/stable")

print(svn("rm ${stable} -m 'stable branch update'"))
print(svn("cp ${trunk} ${stable} -m 'stable branch update'"))



