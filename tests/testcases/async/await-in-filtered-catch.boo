"""
filter saw boom
caught boom
"""
import System
import System.Threading.Tasks

[async] def RunAsync() as Task:
	try:
		raise InvalidOperationException("boom")
	except e as Exception if FilterSaw(e):
		await Task.Delay(1)
		print "caught ${e.Message}"

def FilterSaw(e as Exception) as bool:
	print "filter saw ${e.Message}"
	return true

RunAsync().Wait()
