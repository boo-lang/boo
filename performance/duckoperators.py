import time

def run(name, method):
	start = time.time()
	for i in range(5000000):
		method()		
	print name, time.time()-start
	
def intxint():
	a = 3
	b = 2
	c = a*b

def listxint():
	a = [1, 2, 3]
	b = 2
	c = a*b
	
def dynamicDispatch():
	a = [1, 2, 3]
	a.append(4)
	
run("int*int:", intxint)
run("list*int:", listxint)
run("dynamicDispatch:", dynamicDispatch)
