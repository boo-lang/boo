"""
2007 is the past!
Let's count up to three : 1 2 3
2008 is the future!
Last year was 2007
"""

import System
import System.Threading
import Coroutine

[coroutine(Future: true)]
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
#this will return almost instantaneously (~.3s)
#delay is the delta between the time we've been counting and the duration of computation in the future
#if you add Timeout:0L in the coroutine arguments, you will get a CoroutineFutureNotReadyException
print "Last year was "+LongComputation()
