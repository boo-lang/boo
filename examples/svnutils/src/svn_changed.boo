"""
Print all modified resources in the current repository.
"""
for status in svn_status("."):
	print status.resource if status.code in ("M", "A", "C", "D")
		
