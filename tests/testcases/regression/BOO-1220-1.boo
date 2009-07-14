"""
23
42
"""
import System
import System.Collections.Generic


interface ListInterface (IList[of int]):
	pass

class Impl (List[of int], ListInterface):
	pass

class Impl2 (List[of int], IList[of int]):
	pass


l = Impl()
l.Add(23)
print l[0]
l2 = Impl2()
l2.Add(42)
print l2[0]

