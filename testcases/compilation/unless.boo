"""
condition was false
condition was true
condition was false
"""
def test(condition):
	return "condition was false" unless condition
	return "condition was true"

print(test(false))
print(test(true))
print(test(false))
