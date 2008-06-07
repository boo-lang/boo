"""
Success::System.Collections.IEnumerable.GetEnumerator
Success2::System.Collections.IEnumerable.GetEnumerator
"""

import System.Collections
import System.Collections.Generic

class B:
	pass

class Success(B*):
	def IEnumerable.GetEnumerator() as IEnumerator:
		print "Success::System.Collections.IEnumerable.GetEnumerator"
		return null
	def GetEnumerator() as IEnumerator[of B]:
		return null

#make sure other way around works too
class Success2(B*):
	def GetEnumerator() as IEnumerator[of B]:
		return null
	def IEnumerable.GetEnumerator() as IEnumerator:
		print "Success2::System.Collections.IEnumerable.GetEnumerator"
		return null

e as IEnumerable = Success()
assert null == e.GetEnumerator()
e = Success2()
assert null == e.GetEnumerator()

