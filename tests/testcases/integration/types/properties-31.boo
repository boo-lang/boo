"""
0, 0
1, 1
1, 2: foo
2, 1: bar
"""
import System.Reflection

[DefaultMember("Item")]
class Table:
	Item(i, j):
		get:
			return "${i}, ${j}"
			
		set:
			print "${i}, ${j}: ${value}"
			
class Container:
	Item(i) as Table:
		get:
			return Table()
			
container = Container()
print container.Item[-1][0, 0]
print container.Item[-1].Item[1, 1]
container.Item[0][1, 2] = "foo"
container.Item[1].Item[2, 1] = "bar"
