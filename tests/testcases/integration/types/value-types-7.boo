"""
100x30
200x200
400x200
"""
import System

class Size(ValueType, IComparable):

	public W as int
	public H as int
	
	Area:
		get:
			return W*H
	
	override def ToString():
		return "${W}x${H}"
	
	def CompareTo(other) as int:		
		return Area - cast(Size, other).Area
		
sizes = [Size(W: 200, H: 200), Size(W: 100, H: 30), Size(W: 400, H: 200)]
sizes.Sort()
print join(sizes, "\n")
		

