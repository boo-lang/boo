"""
ping
pong
"""
import System.Threading

def ping(pong as ThreadStart):
	print("ping")
	pong()
	
def pong():
	print("pong")
	
ping(pong)
