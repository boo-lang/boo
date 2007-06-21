"""
I'm a thread!
"""
import System.Threading

t = Thread() do:
	print("I'm a thread!")
	
t.Start()
t.Join()
