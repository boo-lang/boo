"""
yes!
True
False
True
3
True
5
"""
print("no way!") if null or false
print("nah!") if null or false or false
print("yes!") if null or false or true

b = null or false or false
print(b isa bool)
print(b)

b = null or 3 or false
print(b isa int)
print(b)

b = null or false or 5
print(b isa int)
print(b)
