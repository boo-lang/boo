"""
BCE0185-1.boo(11,12): BCE0186: 'greet' has no parameter named 'namex'. Did you mean 'name'?
BCE0185-1.boo(12,15): BCE0185: 'greet' has no parameter named 'zzzzzzzz'.
"""
def greet(name as string, greeting as string = "Hi"):
	print "${greeting}, ${name}!"

# A name the callee has no parameter for is a mistake, not a value to pass by
# position, and a near miss says what was probably meant.
# This test tells us whether a misspelled parameter name is refused.
greet(namex="Jude")
greet(zzzzzzzz="Jude")
