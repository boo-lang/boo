"""
ping
pong
"""
import System.Threading

def ping() as ThreadStart:
	print("ping")
	return pong
	
def pong():
	print("pong")
	
ping()()
