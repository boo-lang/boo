"""
BCE0081-2.boo(7,5): BCE0081: A raise statement with no arguments is not allowed outside an exception handler.
"""
print('start')

try:
	raise
except:
	print('exception')
print('end')

