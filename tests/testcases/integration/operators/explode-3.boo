"""
1
2
Wilson das Neves
"""
callable Function(a, b, *args)

def foo(a, b, args as (object)):
	print a
	print b
	print join(args)
	
f as Function = foo
f(1, 2, *("Wilson", "das", "Neves"))

