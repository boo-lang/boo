import System

def foo():
	print Environment.StackTrace
	
def bar():
	foo()
	
bar()
