#ignore Preference for generic not complete
"""
Good!
"""
import System.Collections
import System.Collections.Generic

class Test(IEnumerable, IEnumerable[of object]):
	def IEnumerable.GetEnumerator():
		print "Bad!"
		return Enumerator()
		
	def GetEnumerator():
		print "Good!"
		return Enumerator()

	class Enumerator(IEnumerator, IEnumerator[of object]):
		pass

for item in Test():
	pass