"""
3
2
0
1 2 3
1 2
"""
callable Function(*args)

def foo(*args):
	print len(args)
	
def bar(args as (object)):
	print join(args)
	
for item in foo, bar:
	f as Function = item
	f(1, 2, 3)
	f(1, 2)
	f()
