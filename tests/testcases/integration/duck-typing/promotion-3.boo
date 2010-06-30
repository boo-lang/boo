class Item:
	[property(value)] _value as int
	
item = Item()
item.value = 1.51

duckyItem as duck = Item()
duckyItem.value = 1.51

assert item.value == duckyItem.value
