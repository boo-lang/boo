def fibonacci():
	a, b = 0, 1
	while true:
		yield b
		a, b = b, a+b
		
def take(count as int, items):
	return item for index, item in zip(range(count), items)

expected = 1, 1, 2, 3, 5, 8

assert expected == array(take(6, fibonacci()))
