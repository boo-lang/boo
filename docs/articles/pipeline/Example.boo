namespace Boo.Examples

class person:	
	fname as string
	lname as string
	
	def constructor(FName, LName as string):
		fname = FName
		lname = LName

	override def ToString():
		return "${lname}, ${fname}"

print(person("Eric", "Idle"))		

