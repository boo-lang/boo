# A closure picks between a macro call and a plain expression by looking at
# the token after the name. A name running straight into the closing brace,
# or into a statement separator, has nothing left to be its argument, so it
# is a value rather than a macro to invoke.
x = 42
byName = { x }
assert 42 == byName()

identity = { p as int | p }
assert 7 == identity(7)

untyped = { p | p }
assert "boo" == untyped("boo")

# The name reached the separator rather than the brace.
separated = { x; }
assert 42 == separated()

# A name with an argument after it is still the macro it always was.
calls = []
appends = { calls.Add("a") }
appends()
appends()
assert 2 == len(calls)

twice = { calls.Add("b"); calls.Add("c") }
twice()
assert 4 == len(calls)

# The guards that were already there still hold: a call, an index, a member
# and a negation are none of them the start of a macro argument.
def three() as int:
	return 3
assert 3 == { three() }()
items = (10, 20)
assert 10 == { items[0] }()
assert 3 == { "abc".Length }()
negated = { -1 }
assert negated() == -1
