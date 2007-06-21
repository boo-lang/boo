a = (array(int, 2), array(int, 2))

for i in range(len(a)):
	for j in range(len(a[i])):
		a[i][j] = (i+1)*(j+1)
		
assert 1 == a[0][0]
assert 2 == a[0][1]
assert 2 == a[1][0]
assert 4 == a[1][1]

assert 4 == a[1][-1]
assert 4 == a[-1][-1]

assert 1 == a[-2][-2]
