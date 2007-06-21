"""
BCE0097-2.boo(5,10): BCE0097: Cannot branch into try block.
"""
try:
	goto start
ensure:
	pass

try:	
	:start
ensure:
	pass
