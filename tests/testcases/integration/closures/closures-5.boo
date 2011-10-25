
predicate = { item as int | return 0 == item % 2 }

assert predicate(2)
assert not predicate(3)
assert predicate(4)

