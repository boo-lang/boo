

a, b = 1, 1
reader = { return a, b }
writer = { a1, b1 | a, b = a1, b1 }

assert array([1, 1]) == (a, b)
assert array([1, 1]) == reader()
writer(2, 3)
assert array([2, 3]) == reader()
assert array([2, 3]) == (a, b)
