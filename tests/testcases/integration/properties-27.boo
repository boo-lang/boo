class MultiplicationTable:
	Item(lhs as int, rhs as int):
		get:
			return lhs*rhs
			
table = MultiplicationTable()
assert 3 == table.Item[3, 1]
assert table.Item[1, 3] == 3
