"""
assert -1 == (-1)
print -1 == (-1)
assert -x == 3
"""
# a paren opening a macro's arguments reads back as its argument list
assert -1 == -1
print -1 == -1
assert -x == 3
