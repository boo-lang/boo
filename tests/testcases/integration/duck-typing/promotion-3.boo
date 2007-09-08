class Item:
	[property(value)] _value as int
	
item = Item() as duck
item.value = 1.51
assert 2 == item.value
