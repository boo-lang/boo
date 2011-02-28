"""
non-generic
42
generic
*42*
generic array
42
"""

import System.Collections.Generic

class Foo:

	def Method[of T](arg as IList of T):
		print "generic"
		return arg[0]
		
	def Method[of T](arg as (T)):
		print "generic array"
		return arg[0]
	
	def Method(arg as IList of int):
		print "non-generic"
		return arg[0]
		
class Bar(Foo):

	def Run():			
		print Method(List[of int]() { 42 })
		print Method(List[of string]() { "*42*" })
		print Method((42,))
		
Bar().Run()
