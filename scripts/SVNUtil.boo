"""
svn utilities
"""
def svn(cmd as string):
	print("svn ${cmd}")
	p = shellp("svn", cmd)
	response = p.StandardOutput.ReadToEnd()	
	p.WaitForExit()
	raise p.StandardError.ReadToEnd() if p.ExitCode
	return response
	
def getTrunkUri():
	return System.Uri(/URL:\s(.+)/.Match(svn("info")).Groups[1].Value)

