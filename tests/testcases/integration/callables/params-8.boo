"""
foo, bar
"""
def foo(values as (string)):
	print join(values, ', ')
	
def printUsing(print as callable(*(string))):
	print("foo", "bar")
	
printUsing foo
