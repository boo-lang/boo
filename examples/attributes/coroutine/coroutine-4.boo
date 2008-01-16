"""
2007 is the past!
Let's count up to three : 1 2 3
The future is not yet ready!
"""

import System
import System.Threading
import Coroutine

[coroutine(Future: true, Timeout: 100L)]
def LongComputation():
	yield 2007
	Thread.Sleep(5s) #long computation
	yield 2008

print LongComputation()+" is the past!" #first call to future is synchronous
Console.Write("Let's count up to three :")
for i in range(1, 4):
	Thread.Sleep(1s)
	Console.Write(" "+i)
Console.Write(Environment.NewLine)
try:
	print LongComputation()+" is the future!"
except e as CoroutineFutureNotReadyException:
	print "The future is not yet ready!"
