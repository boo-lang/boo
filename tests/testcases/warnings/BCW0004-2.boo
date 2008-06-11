"""
BCW0004-2.boo(9,15): BCW0004: WARNING: Right hand side of 'is' operator is a type reference, are you sure you do not want to use 'isa' instead?
BCW0004-2.boo(12,15): BCW0004: WARNING: Right hand side of 'is' operator is a type reference, are you sure you do not want to use 'isa' instead?
"""
import System.Collections
import System.Collections.Generic

l = ArrayList()
assert not (l is ArrayList)

t = List[of string]()
assert not (t is List[of string])

