"""
GenericSelf`1[Leaf]
GenericSelf`2[Leaf,System.Int32]
Narrowed`1[Leaf]
"""
# A derived generic that repeats its base's self constraint and extends it.

abstract class GenericSelf[of T(GenericSelf[of T])]:
	pass

abstract class GenericSelf[of T(GenericSelf[of T]), S(struct)](GenericSelf[of T]):
	pass

abstract class Narrowed[of T(GenericSelf[of T])](GenericSelf[of T]):
	pass

class Leaf(GenericSelf[of Leaf]):
	pass

print typeof(GenericSelf[of Leaf])
print typeof(GenericSelf[of Leaf, int])
print typeof(Narrowed[of Leaf])
