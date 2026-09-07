"""
filter A declined
filter B accepted
B caught boom
"""
import System
import System.Threading.Tasks

[async] def RunAsync() as Task:
	try:
		raise InvalidOperationException("boom")
	except e as Exception if SawA(e):
		await Task.Delay(1)
		print "A caught ${e.Message}"
	except e as Exception if SawB(e):
		await Task.Delay(1)
		print "B caught ${e.Message}"

def SawA(e as Exception) as bool:
	print "filter A declined"
	return false

def SawB(e as Exception) as bool:
	print "filter B accepted"
	return true

RunAsync().Wait()
