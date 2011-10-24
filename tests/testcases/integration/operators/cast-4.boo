def sum(items):
   value = 0
   for item as int in items:
      value += item
   return value

assert 6 == sum([1, 2, 3])
assert 6 == sum([1L, 2L, 3L])
