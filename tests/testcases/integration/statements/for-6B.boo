"""
before
GetEnumerator
MoveNext
Dispose
after
"""
import System
import System.Collections
import System.Collections.Generic

class Enumerable (IEnumerable[of object]):

	class Enumerator(IEnumerator[of object]):
	
		def Dispose():
			print("Dispose")
	
		def Reset():
			print("Reset")
	
		def MoveNext() as bool:
			print("MoveNext")
			return false
	
		Current:
			get:
				print("Current")
				return null

	def GetEnumerator() as IEnumerator[of object]:
		print("GetEnumerator")
		return Enumerator()

	def IEnumerable.GetEnumerator() as IEnumerator:
		return Enumerator()


print("before")
for i in Enumerable():
	print(i)
print("after")
