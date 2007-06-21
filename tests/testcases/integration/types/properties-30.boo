"""
1, 2
2, 1: Carlos Ezequiel
1, 2: Lupa Santiago
"""
import System.Reflection

[DefaultMember("Item")]
class Table:
	Item(i, j):
		get:
			return "${i}, ${j}"
			
		set:
			print "${i}, ${j}: ${value}"
			
table = Table()
print table.Item[1, 2]
table[2, 1] = "Carlos Ezequiel"
table[1, 2] = "Lupa Santiago"
