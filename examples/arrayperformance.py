from time import time

items = 5000000

array = tuple(range(items))

collect = []

start = time() 
for i in xrange(items):
	collect.append(array[i])	
elapsed = time() - start

print elapsed*1000, " elapsed."
