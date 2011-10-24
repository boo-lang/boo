

def adder(amount as int):
	return { value as int | return amount+value }

a1 = adder(3)
a2 = adder(5)

assert 6 == a1(3)
assert 5 == a1(2)
assert 6 == a2(1)
assert 8 == a2(3)
assert a1(4) == a2(2)
