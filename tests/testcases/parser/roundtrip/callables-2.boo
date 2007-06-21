"""
def each(items, action as callable(object)):
	for item in items:
		action(item)

def map(items, function as callable(object) as object):
	return (function(item) for item in items)
"""
def each(items, action as callable(object)):
	for item in items:
		action(item)

def map(items, function as callable(object) as object):
	return function(item) for item in items
