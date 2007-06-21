"""
System.Int32[]
1 2 3
3
System.String[]
 
2
System.Int32[]

0
System.String[]
foo
1
"""
def dump(a):
	print a.GetType()
	print join(a)
	print len(a)
	
a1 = (of int: 1L, 2, 3.1)
dump(a1)

a2 = (of string: null, null)
dump(a2)

a3 = (of int:,)
dump(a3)

a4 = (of string: "foo")
dump(a4)


