"""
1
0
2
0
"""
class Table:
	_table = ((0, 0), (0,0))
	
	Row(index as int) as (object):
		get:
			return _table[index]
	
	def Set(row as int, col as int, value as int):
		_table[row][col] = value
		
t = Table()
t.Set(0, 0, 1)
t.Set(1, 0, 2)

print(t.Row[0][0])
print(t.Row[0][1])
print(t.Row[1][0])
print(t.Row[1][1])

		
		
