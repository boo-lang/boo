"""
True
False
True
True
True
False
"""
o1 = object()
o2 = "foo"

print(o1 is object)
print(o1 is string)
print(o2 is object)
print(o2 is string)
print(o1 is not string)
print(o2 is not string)
