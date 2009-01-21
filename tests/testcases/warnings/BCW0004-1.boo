"""
BCW0004-1.boo(5,12): BCW0004: WARNING: Right hand side of 'is' operator is a type reference, are you sure you do not want to use 'isa' instead?
"""
s = "foo"
b1 = "foo" is string
b2 = s is "bar"
b3 = string is "foo"
t = string
print string is t
print b1, b2, b3 # prevent unused local variable warning

