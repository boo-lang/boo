#ignore Preference for generic not complete
"""
Good!
"""
import System.Collections
import System.Collections.Generic

class Test(IEnumerable, IEnumerable[of int]):
	def IEnumerable.GetEnumerator():
		print "Bad!"
		return Enumerator()
		
	def GetEnumerator():
		print "Good!"
		return Enumerator()

	class Enumerator(IEnumerator, IEnumerator[of int]):
		pass

for item in Test():
	pass