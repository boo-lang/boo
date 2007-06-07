"""
Print all non svn controlled resources.
"""
for resource in svn_locals("."):
	print resource
