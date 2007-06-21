"""
before
GetEnumerator
MoveNext
after
"""
import System
import System.Collections

class Enumerable:

	class Enumerator(IEnumerator):
	
		def Reset():
			print("Reset")
	
		def MoveNext() as bool:
			print("MoveNext")
			return false
	
		Current:
			get:
				print("Current")
				return null

	def GetEnumerator() as IEnumerator:
		print("GetEnumerator")
		return Enumerator()
		
print("before")
for i in Enumerable():
	print(i)
print("after")
