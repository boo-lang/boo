from time import time

def value(x, y, z):
	return 2**x * 3**y * 5**z

def ugly(maxValue):
	uglies = []
	counter = 1
	d = {1 : (0, 0, 0)}

	while len(uglies) < maxValue:	
		uglies.append(counter)
		x, y, z = d[counter]
		
		d[value(x+1, y, z)] = (x+1, y, z)
		d[value(x, y+1, z)] = (x, y+1, z)
		d[value(x, y, z+1)] = (x, y, z+1)
		
		del d[counter]
		
		keys = d.keys()
		keys.sort()		
		counter = keys[0]
	
	return uglies[-1]
	
iterations = 1500

start = time()
for i in range(10):
	uvalue = ugly(iterations)
end = time()
print iterations, "ugly value =", uvalue, "in", (end - start)*1000, "ms" 

