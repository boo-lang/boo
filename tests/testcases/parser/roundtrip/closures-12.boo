"""
a = [1, 2, 3].Find({ item as int | return (item > 2) })
"""
a = [1, 2, 3].Find() do (item as int):
	return item > 2

