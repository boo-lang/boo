def run(name, method as callable()):
	start = date.Now
	for i in range(1000000):
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
	
run("int*int:", intxint)
run("list*int:", listxint)
