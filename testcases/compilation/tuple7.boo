"""
1, um
2, dois
"""
numbers = (1, 2)
text = ("um", "dois")
for n, t in zip(numbers, text):
	print("${n}, ${t}")
