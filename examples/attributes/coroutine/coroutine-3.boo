"""
2007 is the past!
Let's count up to three : 1 2 3
2008 is the future!
OK! Terminated!
"""

import System
import System.Threading
import Coroutine

[coroutine(Future: true, Looping: false)]
def LongComputation():
	yield 2007
	Thread.Sleep(3.3s) #long computation
	yield 2008

print LongComputation()+" is the past!" #first call to future is synchronous
Console.Write("Let's count up to three :")
for i in range(1, 4):
	Thread.Sleep(1s)
	Console.Write(" "+i)
Console.Write(Environment.NewLine)
print LongComputation()+" is the future!"
try:
	LongComputation()
except e as CoroutineTerminatedException:
	print "OK! Terminated!"
