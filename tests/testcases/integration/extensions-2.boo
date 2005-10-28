"""
op_Equality(self as string, rhs as char)
True
op_Equality(self as string, rhs as char)
False
op_Equality(self as char, rhs as string)
op_Equality(self as string, rhs as char)
True
"""
internal def op_Equality(self as string, rhs as char):
	print "op_Equality(self as string, rhs as char)"
	return len(self) == 1 and self[0] == rhs
	
internal def op_Equality(self as char, rhs as string):
	print "op_Equality(self as char, rhs as string)"
	return rhs == self
	
print "a" == char('a')
print "ab" == char('a')
print char('a') == "a"

