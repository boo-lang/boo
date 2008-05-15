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
		def MoveNext():
			return false
		
		IEnumerator.Current:
			get: return object();
		
		Current as object:
			get: return object();
		
		def Reset():
			pass
		
		def Dispose():
			pass

for item in Test():
	pass