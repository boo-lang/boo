a = array(byte, range(3, 0))
b = (cast(byte, 3), cast(byte, 2), cast(byte, 1))

assert a == b
assert a == array(byte, [3, 2, 1])

c = array(double, range(3, 0))
d = (3.0, 2.0, 1.0)

assert c == d
assert c == array(double, [3, 2, 1])
