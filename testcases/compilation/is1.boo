"""
True
False
True
True
True
False
True
False
True
"""
o1 = object()
o2 = "foo"

print(o1 is object)
print(o1 is string)
print(o2 is object)
print(o2 is string)
print(o1 is not string)
print(o2 is not string)

t1 = int
t2 = string

# (System.Type is System.Type) maps to reference comparison
print(t1 is int)
print(t1 is string)
print(t2 is string)
