"""
System.Int32
System.Int32
System.Int32
System.Int32
System.Int32
"""
def pet(o):
	print o.GetType().GetElementType()
	
def returnArray() as int*:
	return (,)
	
def yieldArray() as int**:
	yield (,)
	
o as int* = (,)
pet(o)
pet(returnArray())
for a in yieldArray(): pet(a)
pet(cast(int*, (,)))
pet((,) as int*)

