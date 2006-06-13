"""
True
False
True
True
True
True
True
False
True
"""
o1 = object()
o2 = "foo"

# pattern (object is TypeReference) maps to type test
print(o1 isa object)
print(o1 isa string)
print(o2 isa object)
print(o2 isa string)

t1 = int
t2 = string

# making sure is compares references
print(t1 isa System.Type)
print(t2 isa System.Type)
print(t1 is int)
print(t1 is string)
print(t2 is string)
