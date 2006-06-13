import System

def foo():
	yield cast((int, 2), Array.CreateInstance(int, 2, 2))

// test type inference
for a in foo():
	value = 0
	for i in range(2):
		for j in range(2):
			a.SetValue(++value, i, j)
			assert value == a.GetValue(i, j)
