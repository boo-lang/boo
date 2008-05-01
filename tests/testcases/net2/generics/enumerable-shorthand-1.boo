"""
System.Collections.Generic.IEnumerable`1[System.Int32]
System.Collections.Generic.IEnumerable`1[System.Nullable`1[System.Int32]]
1
2
3
"""
def CountToThree() as int*:
	yield 1
	yield 2
	yield 3

def PrintAll(ints as int*):
	for i in ints:
		print i

print typeof(int*)
print typeof(int?*)
PrintAll(CountToThree())

