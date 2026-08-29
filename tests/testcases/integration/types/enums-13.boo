"""
0
1
Item1
Item2
"""
enum E:
	Item1
	Item2
	
print cast(int, E.Item1)
print cast(int, E.Item2)
print cast(E, 0)
print cast(E, 1)
