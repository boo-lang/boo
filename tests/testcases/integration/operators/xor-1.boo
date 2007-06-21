assert true == (true ^ false)
assert true == (false ^ true)
assert false == (true ^ true)
assert false == (false ^ false)

assert 1 == (0xFF ^ 0xFE)
assert 1 == (1 ^ 0)
assert 0 == (1 ^ 1)
