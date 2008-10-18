"""
42
"""

externalValue = 42
	
def rec(n as int) as int:
	return rec(++n) if n < externalValue
	return n
		
print rec(0)

	
