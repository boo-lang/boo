generator = { yield 1; yield 2 }
e = generator().GetEnumerator()
item, = e
assert 1 == item
item, = e
assert 2 == item
