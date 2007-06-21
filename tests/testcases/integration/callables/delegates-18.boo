"""
Hello!
"""
import System.Threading

class T:
	def constructor(start as ThreadStart):
		start()

T() do:
	print("Hello!")
