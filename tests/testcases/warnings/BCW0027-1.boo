"""
BCW0027-1.boo(6,5): BCW0027: WARNING: Obsolete syntax 'Item(index)'. Use 'Item[index]'.
BCW0027-1.boo(8,5): BCW0027: WARNING: Obsolete syntax 'Item(index1, index2 as int)'. Use 'Item[index1, index2 as int]'.
"""
class Obsolete:
	Item(index):
		get: return null
	Item(index1, index2 as int):
		get: return null

