# a lone name in a closure is its value, not a macro to invoke
x = 42
byName = { x }
assert 42 == byName()

identity = { p as int | p }
assert 7 == identity(7)

untyped = { p | p }
assert "boo" == untyped("boo")

separated = { x; }
assert 42 == separated()

# a name with an argument after it is still a macro
calls = []
appends = { calls.Add("a") }
appends()
appends()
assert 2 == len(calls)

twice = { calls.Add("b"); calls.Add("c") }
twice()
assert 4 == len(calls)

def three() as int:
	return 3
assert 3 == { three() }()
items = (10, 20)
assert 10 == { items[0] }()
assert 3 == { "abc".Length }()
negated = { -1 }
assert negated() == -1
