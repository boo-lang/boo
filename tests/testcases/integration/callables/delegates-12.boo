"""
before.
Hello!
after.
"""
import System.Threading

class Printer:
	_message as string
	
	def constructor(message):
		_message = message
		
	def print():
		System.Console.WriteLine(_message)
		
def run(start as ThreadStart):
	print("before.")
	start()
	print("after.")
	
run(Printer("Hello!").print)
