def fibonacci():
	a, b = 0, 1
	while true:
		yield b
		a, b = b, a+b

for index as int, element in zip(range(5), fibonacci()):
	print("${index+1}: ${element}")
