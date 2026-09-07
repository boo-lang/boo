"""
A T
"""
import System
import System.Threading.Tasks

[async] def RunAsync() as Task:
	tag = "T"
	try:
		raise InvalidOperationException("boom")
	except as Exception if tag.Length > 0:
		await Task.Delay(1)
		print "A ${tag}"
	except as Exception if tag.Length > 1:
		await Task.Delay(1)
		print "B ${tag}"

RunAsync().Wait()
