"""
BCE0081-3.boo(10,1): BCE0081: A raise statement with no arguments is not allowed outside an exception handler.
"""
print('start')

try:
	raise "an exception"
except:
	print('exception')
raise
print('end')

