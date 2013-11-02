"""
f1
f2
"""
def f1():
	print 'f1'

def f2():
	print 'f2'

def invoke(value):
	(f1 if value else f2)()

invoke true
invoke false
