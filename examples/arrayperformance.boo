array = List(range(1000)).ToArray(int)

collect = []

start = date.Now
for i in range(1000):
	collect.Add(array[i])
	
elapsed = date.Now.Subtract(start)
print("${elapsed.TotalMilliseconds} elapsed.")
