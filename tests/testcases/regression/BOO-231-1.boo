a as object = 0.0
b as object = 1.0

assert true == (not a)
assert false == (not b)
assert false == ((not b) == (not a))
assert true == (not a)

