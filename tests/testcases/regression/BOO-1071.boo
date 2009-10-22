// Verify that list comprehension works inside an array constructor 
l = [i ** 2 for i in range(10)]
a1 = array(short, l)
a2 = array(short, [i ** 2 for i in range(10)])
assert a1 == a2

