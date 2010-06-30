class Item:
	[property(value)] _value as char
	
item = Item()
item.value = 32

duckyItem as duck = Item()
duckyItem.value = 32

assert item.value == duckyItem.value
