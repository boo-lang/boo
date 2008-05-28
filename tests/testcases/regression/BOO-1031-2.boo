#ignore BOO-1031 may require extensive refactoring of ProcessInheritedAbstractMembers
"""
BOO-1031-2.boo(13,9): BCE0035: 'Fail1.GetEnumerator' conflicts with inherited member 'System.Collections.IEnumerable.GetEnumerator'.
BOO-1031-2.boo(21,9): BCE0035: 'Fail2.GetEnumerator' conflicts with inherited member 'System.Collections.Generic.IEnumerable`1[B].GetEnumerator'.
"""

import System.Collections
import System.Collections.Generic

class B:
	pass

class Fail1(B*):
	def IEnumerable.GetEnumerator() as B*:
		return null
	def GetEnumerator() as IEnumerator[of B]:
		return null

class Fail2(B*):
	def IEnumerable.GetEnumerator() as IEnumerator:
		return null
	def GetEnumerator() as B*:
		return null

