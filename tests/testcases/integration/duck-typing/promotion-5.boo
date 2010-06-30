class Item:
	[property(value)] _value as int
	
value = char('a')

item = Item()
item.value = value

duckyItem as duck = Item()
duckyItem.value = value

assert item.value == duckyItem.value
