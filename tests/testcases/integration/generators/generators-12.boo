
g = i*2 for i in range(5)

current = 0
for i in g:
	assert current == i/2, "integer division"
	++current
