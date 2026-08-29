"""
before generator
after generator
before iteration
gen
0
1
after iteration
"""
def gen():
	print("gen")
	return range(2)
	
print("before generator")
a = i for i in gen()
print("after generator")

print("before iteration")
for i in a:
	print(i)
print("after iteration")
