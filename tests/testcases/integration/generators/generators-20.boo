"""
1, 3
1, 2
1, 1
2, 3
2, 2
2, 1
"""
items = [(x, y) for x in range(1, 3) for y in range(3, 0)]
assert items isa List
for item in items:
	print join(item, ", ")
