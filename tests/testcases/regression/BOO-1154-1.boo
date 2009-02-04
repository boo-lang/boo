"""
42
"""
import System.Collections
import System.Collections.Generic

class IntCollection (ICollection[of int]):
	Count as int:
		get: return 42

	def IEnumerable.GetEnumerator():
		raise System.NotImplementedException()

c = IntCollection()
print len(c)

