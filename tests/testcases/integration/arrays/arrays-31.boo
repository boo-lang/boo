import System

a as (int, 2) = Array.CreateInstance(int, 2, 2)

value = 0
for i in range(2):
	for j in range(2):
		a.SetValue(++value, i, j)
		assert value == a.GetValue(i, j)
