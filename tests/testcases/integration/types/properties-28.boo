import System.Reflection

[DefaultMember("Item")]
class MultiplicationTable:
	Item(lhs as int, rhs as int):
		get:
			return lhs*rhs
			
table = MultiplicationTable()
assert 3 == table[3, 1]
assert table[1, 3] == 3
