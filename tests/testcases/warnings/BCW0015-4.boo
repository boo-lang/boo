"""
"""

def Test():
	goto reachable_goto
	:reachable_goto
	print "reachable"
	return
	:unreachable_goto #this is not handled yet -> BOO-457
	print "unreachable 2"

Test()
