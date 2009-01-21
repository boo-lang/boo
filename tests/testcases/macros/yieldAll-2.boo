"""
4
8
15
16
23
42
"""
import System.Collections.Generic

def ElementsFromLargerListFirst(a as IList[of int], b as IList[of int]):
	if a.Count > b.Count:
		yieldAll a, b
	else:
		yieldAll b, a


a = List[of int]()
a.Add(23)
a.Add(42)
b = List[of int]()
b.Add(4)
b.Add(8)
b.Add(15)
b.Add(16)

for i in ElementsFromLargerListFirst(a, b):
	print i

