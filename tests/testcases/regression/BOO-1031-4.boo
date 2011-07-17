"""
'Fail.GetEnumerator()' conflicts with inherited member 'System.Collections.Generic.IEnumerable[of B].GetEnumerator()'.
'Fail2.GetEnumerator()' conflicts with inherited member 'System.Collections.Generic.IEnumerable[of B].GetEnumerator()'.
"""

import Boo.Lang.Compiler.MetaProgramming

code = [|

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

|]

for error in compile_(code).Errors:
	print error.Message