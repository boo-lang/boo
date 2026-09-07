"""
filter declined
outer caught boom
"""
import System
import System.Threading.Tasks

[async] def RunAsync() as Task:
	try:
		try:
			raise InvalidOperationException("boom")
		except e as Exception if Declines(e):
			await Task.Delay(1)
			print "inner caught ${e.Message}"
	except e2 as Exception:
		print "outer caught ${e2.Message}"

def Declines(e as Exception) as bool:
	print "filter declined"
	return false

RunAsync().Wait()
