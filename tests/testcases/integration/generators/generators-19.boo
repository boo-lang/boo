"""
1, 5
1, 3
1, 1
3, 5
3, 3
3, 1
5, 5
5, 3
5, 1
"""
items = (x, y) for x in range(1, 6) if (x % 2) for y in range(5, 0) if y % 2
for item in items:
	print join(item, ", ")
