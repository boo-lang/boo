"""
0
1
finally
inner finally
caught inner
outer finally
"""
import System
import System.Threading.Tasks

[async] def BreakAsync() as Task:
	try:
		for i in range(5):
			if i == 2:
				break
			print i
	ensure:
		await Task.Delay(1)
		print "finally"

[async] def NestedAsync() as Task:
	try:
		try:
			raise InvalidOperationException("inner")
		ensure:
			await Task.Delay(1)
			print "inner finally"
	except e as Exception:
		print "caught ${e.Message}"
	ensure:
		await Task.Delay(1)
		print "outer finally"

BreakAsync().Wait()
NestedAsync().Wait()
