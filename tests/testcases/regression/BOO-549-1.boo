"""
1
"""
class X:
	def T(params as int) as System.Collections.IEnumerable:
		return Y.T()

class Y:
	public static T = def():
		yield 1

g = X().T(1).GetEnumerator()
item, = g
print item
assert not g.MoveNext()


