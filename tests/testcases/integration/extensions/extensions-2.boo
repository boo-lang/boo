"""
op_Equality(self as string, rhs as char)
True
op_Equality(self as string, rhs as char)
False
op_Equality(self as char, rhs as string)
op_Equality(self as string, rhs as char)
True
"""
[Extension]
internal def op_Equality(lhs as string, rhs as char):
	print "op_Equality(self as string, rhs as char)"
	return len(lhs) == 1 and lhs[0] == rhs
	
[Extension]
internal def op_Equality(lhs as char, rhs as string):
	print "op_Equality(self as char, rhs as string)"
	return rhs == lhs
	
print "a" == char('a')
print "ab" == char('a')
print char('a') == "a"

