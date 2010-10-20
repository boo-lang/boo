"""
System.String
System.Int32
System.String
System.Int32
System.String
System.Int32
System.Int32
"""
def pa(a):
	print a.GetType().GetElementType()
	
def withVarArgs(o as (string), *a as (int)):
	pa(o)
	pa(a)
	
def withArrayVarArgs(o as (string), *aa as ((int))):
	pa(o)
	for a in aa: pa(a)
	
withVarArgs((,))
withVarArgs((,), *(,))
withArrayVarArgs((,), (,), (,))
