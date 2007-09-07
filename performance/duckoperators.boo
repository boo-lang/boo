import Boo.Lang.Runtime

def benchmark(label, method as callable()):
	start = date.Now
	for i in range(5000000):
		method()		
	print "${label}\t", (date.Now-start).TotalSeconds
	
class ListExtensions:
	[extension]
	static def addX(l as List, value):
		l.Add(value)
		
	[extension]
	static length[l as List]:
		get: 
			return l.Count

benchmark "int*int":
	a as duck = 3
	b as duck = 2
	c = a*b
	
benchmark "static int*int":
	a = 3
	b = 2
	c = a*b
	
benchmark "List*int":
	a as duck = [1, 2, 3]
	b as duck = 2
	c = a*b
	
benchmark "static List*int":
	a = [1, 2, 3]
	b = 2
	c = a*b

benchmark "List.Add":
	b as duck = [1, 2, 3]
	b.Add(4)

RuntimeServices.WithExtensions(ListExtensions):
	benchmark "extension List.Add":
		b as duck = [1, 2, 3]
		b.addX(4)
		
benchmark "static List.Add":
	b = [1, 2, 3]
	b.Add(4)
	
benchmark "property":
	b as duck = []
	c = b.Count
	
RuntimeServices.WithExtensions(ListExtensions):
	benchmark "extension property":
		b as duck = []
		c = b.length
	
benchmark "static property":
	b = []
	c = b.Count
	

