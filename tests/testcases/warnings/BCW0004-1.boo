"""
BCW0004-1.boo(4,12): BCW0004: WARNING: right hand side of 'is' operator is a type reference, are you sure you don't want to use 'isa' instead?
"""
b1 = "foo" is string
b2 = "foo" is "bar"
b3 = string is "foo"
t = string
print string is t
print b1, b2, b3 # prevent unused local variable warning
