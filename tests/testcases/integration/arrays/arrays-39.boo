"""
1
0
2
0
"""
import System
import System.Collections
import System.Reflection

[DefaultMember("Item")]
[EnumeratorItemType(int)]
class Table:
	_table = matrix(int, 2, 2)

	Item(x as int, y as int):
		get:
			return _table[x,y]
		set:
			_table[x,y] = value

t = Table()
t[0,0]=1
t[1,0]=2

print(t[0,0])
print(t[0,1])
print(t[1,0])
print(t[1,1])
