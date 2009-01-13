"""
2
1
9
2
5
"""

def Foo():
	yield 1
	yield 2

x = 0
for x in Foo():
	pass
print x

for x in Foo():
	break if x == 1
print x

j = 0
for j in range(10):
	pass
print j

for j in range(10, 0, -2):
	pass
print j

for j in (1, 1, 2, 3, 5):
	pass
print j

