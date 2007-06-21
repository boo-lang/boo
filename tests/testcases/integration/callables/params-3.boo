"""
0: 
2: 1, 2
3: 1, 'foo', 'baz'
1: ['foo', 'bar']
1: [1, 2]
"""
def foo(*args):
	print len(args) + ":", join(repr(item) for item in args, ', ')
	
def repr(o) as object:
	if o isa string:
		return "'${o}'"
	if o isa System.Collections.IList:
		return "[${join(repr(item) for item in o, ', ')}]"
	return o

foo()	
foo(1, 2)
foo(1, "foo", "baz")
foo(array(object, ["foo", "bar"]))
foo(array(int, [1, 2]))
