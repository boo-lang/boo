def test(s as string):
	return char('f') in s

def benchmark():
	start = date.Now
	for i in range(1, 1000000):
		test "dsd"
	print date.Now - start, "elapsed"
	
for i in range(3):
	benchmark()
