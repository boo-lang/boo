"""
assert -1 == (-1)
print -1 == (-1)
assert -x == 3
"""
# A paren opening a macro's arguments reads back as its argument list, so
# "assert (-1) == (-1)" would return from the parser as a call to assert.
assert -1 == -1
print -1 == -1
assert -x == 3
