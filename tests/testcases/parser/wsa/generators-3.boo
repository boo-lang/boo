"""
a = ((x, y) for x in (1, 2, 3) for y in (4, 5, 6))
b = [((x, y) for x in range(4) if (x % 2) for y in range(5) if (y % 2))]
"""
a = ((x, y) for x in (1, 2, 3) for y in (4, 5, 6))
b = [((x, y) for x in range(4) if (x % 2) for y in range(5) if (y % 2))]
