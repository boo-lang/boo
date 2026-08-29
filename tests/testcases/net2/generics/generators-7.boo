"""
Thingy1
Thingy2
Thingy3
Thingy4
Thingy0
Thingy5
"""

import System.Collections.Generic

def Yield() as IEnumerable of long:
	yield 1
	yield 2
	yield 3
	yield 4.0
	yield
	yield 5

for i in Yield(): print "Thingy" + i

