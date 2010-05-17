"""
i as long = [1, 2, 3].IndexOf({ item as int | return (item > 2) })
"""
i as long = [1, 2, 3].IndexOf() do (item as int):
	return item > 2
