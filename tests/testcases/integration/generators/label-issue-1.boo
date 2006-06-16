"""
foo
baz
"""
def foo():
	for first, second in (("foo", "b"), ("a", "b"), ("c", "baz")):
		if len(first) < 2 and len(second) < 2: continue
		if len(first) > 2:		
			yield first, second
		elif len(second) > 2:
			yield second, first
			
for first, second in foo():
	print first
