"""
BCE0145-1.boo(6,11): BCE0145: Cannot catch type 'int'; 'except' blocks can only catch exceptions derived from 'System.Exception'. To catch non-CLS compliant exceptions, use a default exception handler or catch 'System.Runtime.CompilerServices.RuntimeWrappedException'.
"""
try:
	print '!'
except as int:
	print "Caught Int!"
