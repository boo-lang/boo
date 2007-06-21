"""
Foo #1
Foo #2
Foo #3
Foo #4
"""

import System

class Foo:
	public _i as int

	public def constructor(i as int):
		_i = i	

	public def ToString():
		return "Foo #${_i}";

ints = (1,2,3,4)
strings = Array.ConvertAll[of int, Foo](ints, {i as int | Foo(i)})
for s in strings: print s
