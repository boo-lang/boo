def test():
	items = 2000000
	
	array as (object) = List(range(items)).ToArray(object)
	
	collect = []
	
	start = date.Now
	for i as int in range(items):
		collect.Add(array[i])	
	elapsed = date.Now.Subtract(start)
	
	print("${elapsed.TotalMilliseconds} elapsed.")
	
test()
test()
test()
