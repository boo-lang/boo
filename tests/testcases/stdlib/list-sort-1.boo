

l = [2, 4, 3, 5, 1]
assert l.Sort() == [1, 2, 3, 4, 5]
assert l.Sort({ lhs as int, rhs as int | return rhs - lhs }) == [5, 4, 3, 2, 1]
