def time(description, arg, closure as callable(object)):
	iterations = 1000000
	start = date.Now
	for i in range(iterations):
		closure(arg)
	elapsed = date.Now-start
	print("${description}: ${elapsed}")
	print("${description}: ${elapsed.TotalMilliseconds/iterations} ms per call")
	
def foo():
	pass
	
time("simple method call", null) do (item):
	foo()

time("method reference", foo) do (item as callable()):
	item()
	
time("interface call", foo) do (item as callable):
	item()

time("adapted reference", foo) do (item as callable()):
	cast(callable(object), item)(null)
