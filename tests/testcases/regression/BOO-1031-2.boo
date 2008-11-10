"""
BCE0035: 'Fail1.GetEnumerator' conflicts with inherited member 'System.Collections.IEnumerable.GetEnumerator'.
BCE0035: 'Fail2.GetEnumerator' conflicts with inherited member 'System.Collections.Generic.IEnumerable[of B].GetEnumerator()'.
"""

import Boo.Lang.Compiler.MetaProgramming

code = [|

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

|]

print compile_(code).Errors