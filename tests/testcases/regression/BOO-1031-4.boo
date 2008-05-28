#ignore BOO-1031 may require extensive refactoring of ProcessInheritedAbstractMembers
"""
BOO-1031-4.boo(15,9): BCE0035: 'Fail.GetEnumerator' conflicts with inherited member 'System.Collections.Generic.IEnumerable`1[B].GetEnumerator'.
BOO-1031-4.boo(19,9): BCE0035: 'Fail2.GetEnumerator' conflicts with inherited member 'System.Collections.IEnumerable.GetEnumerator'.
"""

import System.Collections
import System.Collections.Generic

class B:
	pass

class Fail(B*):
	def IEnumerable.GetEnumerator():
		return null
	def GetEnumerator() as B*:
		return null

class Fail2(B*):
	def GetEnumerator() as B*:
		return null
	def IEnumerable.GetEnumerator():
		return null

