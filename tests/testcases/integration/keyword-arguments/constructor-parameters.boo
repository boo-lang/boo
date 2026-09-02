"""
1,2
1,2
1,2
16
"""
import System.Text

class Point:
	public X as int
	public Y as int

	def constructor(x as int, y as int):
		X = x
		Y = y

	override def ToString() as string:
		return "${X},${Y}"

# A constructor takes names like any other call, in any order, and an external
# one does too. The 'Name: value' form still sets a property after
# construction, which is a different thing and stays as it was.
# This test tells us whether naming a constructor's parameter reaches it.
print Point(1, 2)
print Point(x=1, y=2)
print Point(y=2, x=1)
print StringBuilder(capacity=16).Capacity
