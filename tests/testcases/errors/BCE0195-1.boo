"""
BCE0195-1.boo(10,17): BCE0195: Exception variable 'e' cannot be used in a filtered catch block that awaits.
"""
import System
import System.Threading.Tasks

[async] def Run() as Task:
	try:
		raise InvalidOperationException("boom")
	except e as Exception if e.Message == "boom":
		await Task.Delay(1)
		print e.Message
