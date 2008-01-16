"""
Apply [coroutine] to a method is the 1st step
??? is the 2nd step
PROFIT is the 3rd step
"""

import System
import System.Threading
import Coroutine

[coroutine]
def Step():
	Console.Write("Apply [coroutine] to a method")
	yield 1
	Console.Write("???")
	yield 2
	Console.Write("PROFIT")
	yield 3

print " is the ${Step()}st step"
print " is the ${Step()}nd step"
print " is the ${Step()}rd step"
