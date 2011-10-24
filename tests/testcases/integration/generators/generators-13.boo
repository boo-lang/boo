
def foo():
	return i*2 for i in range(5)

current = 0
for i in foo():
	assert current == i/2, "integer division"
	++current
