#ignore break and continue out of an awaiting finally skip the finally body
"""
0
finally 0
finally 1
2
finally 2
0
finally 0
after
"""
import System.Threading.Tasks

[async] def ContinueAsync() as Task:
	for i in range(3):
		try:
			if i == 1:
				continue
			print i
		ensure:
			await Task.Delay(1)
			print "finally ${i}"

[async] def BreakAsync() as Task:
	for i in range(3):
		try:
			print i
			break
		ensure:
			await Task.Delay(1)
			print "finally ${i}"
	print "after"

ContinueAsync().Wait()
BreakAsync().Wait()
