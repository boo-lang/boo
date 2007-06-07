"""
Iterate through every non svn controlled resource asking if it
should be added to svn:ignore.
"""
import System.IO

for resource in svn_locals("."):
	path = Path.GetDirectoryName(resource).Replace("\\", "/")
	what = Path.GetFileName(resource)
	
	if confirm("ignore '${what}' in path '${path}'?"):
		svn_ignore(path, what)

