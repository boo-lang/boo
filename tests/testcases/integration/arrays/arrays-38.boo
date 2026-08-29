"""
1
0
2
0
"""

class Table:
	_table = matrix(int, 2, 2)

	Item(x as int, y as int):
		get:
			return _table[x,y]
		set:
			_table[x,y] = value

t = Table()
t.Item[0,0]=1
t.Item[1,0]=2

print(t.Item[0,0])
print(t.Item[0,1])
print(t.Item[1,0])
print(t.Item[1,1])
