"""
0 4 8 12 16
0 4 8 12 16
"""

generator = i*2 for i in range(10) if 0 == i % 2

e1 = generator.GetEnumerator()
e2 = generator.GetEnumerator()
assert e1 is not e2, "IEnumerable instances must be distintic!"

for i in 0, 4, 8, 12, 16:
	assert e1.MoveNext()
	assert e2.MoveNext()
	assert i == e1.Current
	assert i == e2.Current

print(join(generator))
print(join(generator))

