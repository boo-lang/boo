"""
True
False
True
"""
internal def op_Equality(self as string, rhs as char):
	return len(self) == 1 and self[0] == rhs
	
internal def op_Equality(self as char, rhs as string):
	return rhs == self
	
print "a" == char('a')
print "ab" == char('a')
print char('a') == "a"
