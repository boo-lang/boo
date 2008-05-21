"""
not null
"""
def foo(block as callable()):
	if block is null: return
	print "not null"
	
foo:
	pass
