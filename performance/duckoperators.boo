def run(name, method as callable()):
	start = date.Now
	for i in range(5000000):
		method()		
	print name, (date.Now-start).TotalSeconds
	
def intxint():
	a as duck = 3
	b as duck = 2
	c = a*b

def listxint():
	a as duck = [1, 2, 3]
	b as duck = 2
	c = a*b
	
def dynamicDispatch():
	b as duck = [1, 2, 3]
	b.Add(4)
	
def staticDispatch():
	b = [1, 2, 3]
	b.Add(4)
	
run("int*int:", intxint)
run("list*int:", listxint)
run("dynamicDispatch:", dynamicDispatch)
run("staticDispatch", staticDispatch)
