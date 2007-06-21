"""
97
97
a
a
"""
i1 = cast(int, char('a'))
print i1

i2 = cast(int, cast(object, char('a')))
print i2

c1 = cast(char, i2)
print c1

c2 = cast(char, cast(object, i2))
print c2
