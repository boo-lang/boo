"""
2
4
6
0
"""

import System.Collections.Generic

def YieldInts() as IEnumerable of int:
	yield 1
	yield 2
	yield 3
	yield

for i in YieldInts(): print i * 2
