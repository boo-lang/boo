"""
[Extension]
static def foo(item as string):
	return item.ToUpper()

[Extension]
static def join(item as string, items):
	return join(items, self)
"""
[Extension]
static def foo(item as string):
	return item.ToUpper()
	
[Extension]
static def join(item as string, items):
	return join(items, self)
