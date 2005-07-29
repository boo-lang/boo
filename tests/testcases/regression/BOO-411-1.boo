"""
True
"""
class Id:
	id as string

	def constructor( newId as string ):
		if newId == null:
			raise "Id: is null parameter"
		id = newId

	def Equals( x as object ):
		return (self == x)
		
	static def op_Equality( left as Id, right as duck ):
		if right isa Id:
			rightId = right as Id
			return rightId.id.Equals( left.id )
		return false
		
	def GetHashCode():
		return id.GetHashCode()
		
print Id("A") == Id("A")
