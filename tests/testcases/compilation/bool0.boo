"""
foo
baz
"""
def say(msg, condition):
	print(msg) if condition
	
say("foo", true)
say("bar", false)
say("baz", 1)
say("spam", null)

