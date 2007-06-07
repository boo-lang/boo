"""
svn utility functions
"""
import System
import System.IO

class ResourceStatus:
	public code as string
	public resource as string

def svn_status(resource as string):
	for line in lines(shell("svn", "status ${resource}")):
		yield ResourceStatus(code: line[:7].Trim(), resource: line[7:])

def svn_pg(resource as string, propertyName as string):
	return shell("svn", "pg ${propertyName} ${resource}")
	
def svn_ps(resource as string, propertyName as string, propertyValue as string):
	tempFile = ".svn_ignore"
	File.WriteAllText(tempFile, propertyValue)
	try:
		return shell("svn", "ps ${propertyName} --file \"${tempFile}\" ${resource}")
	ensure:
		File.Delete(tempFile)
		
def svn_locals(resource as string):
	return (
		status.resource
		for status in svn_status(resource)
		if status.code == "?")
		
def svn_ignore(resource as string, whatToIgnore as string):
	current = svn_pg(resource, "svn:ignore")
	print svn_ps(resource, "svn:ignore", current.Trim() + "\n" + whatToIgnore)
		
def lines(s as string):
	return line.Trim() for line in /(\r?\n)+/.Split(s) if len(line.Trim())
	
def confirm(message as string):
	return "y" == prompt("${message} (y/n): ")

