"""
1, 1
home
"""
for i in 1, 2, 3:
	for j in 1, 2, 3:
		print("${i}, ${j}")
		goto home
:home
print("home")
