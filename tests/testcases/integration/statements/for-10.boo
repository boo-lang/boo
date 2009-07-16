"""
foofoo
foobar
1
2
3
4
"""
import System.Collections.Generic

def GenericParameterIterator[of T](l as List[of T]):
	for i in l:
		if i isa string:
			print "foo${i}"
		else:
			print i

def GenericParameterValueTypeIterator[of T (struct)](l as List[of T]):
	for i in l:
		print i

GenericParameterIterator(List[of string](("foo","bar")))
GenericParameterIterator(List[of int]((1,2)))
GenericParameterValueTypeIterator(List[of int]((3,4)))

