"""
System.Object
System.Object
System.Object
System.Object
System.Object
"""
def pet(o):
	print o.GetType().GetElementType()
	
def returnArray() as object*:
	return (,)
	
def yieldArray() as object**:
	yield (,)
	
o as object* = (,)
pet(o)
pet(returnArray())
for a in yieldArray(): pet(a)
pet(cast(object*, (,)))
pet((,) as object*)

