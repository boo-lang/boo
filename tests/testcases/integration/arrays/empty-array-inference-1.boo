"""
System.String
System.Object
System.Int32
System.String
System.String[]
System.String
System.String
System.Int32
System.Boolean
System.Int64
"""
def pa(a):
	print a.GetType().GetElementType()
	
def useStrings(a as (string)):
	pa(a)
	
def useIntegers(a as (int)):
	pa(a)
	
def returnBooleans() as (bool):
	return (,)
	
def yieldLongs() as (long)*:
	yield (,)
	
a1 as (string) = (,)
pa(a1)

a2 as (object) = (,)
pa(a2)

a3 as (int) = (,)
pa(a3)

a4 = (,) as (string)
pa(a4)

a5 = (,) as ((string))
pa(a5)

a6 = cast((string), (,))
pa(a6)

useStrings((,))
useIntegers((,))
pa(returnBooleans())
for longs in yieldLongs():
	pa(longs)
