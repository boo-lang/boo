import Generators from BooModules

current = 1
for i in oddNumbers(10):
	assert 1 == i % 2, "integer modulus"
	assert current == i
	current += 2
