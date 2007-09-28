"""
foo bar
"""
def lines(s as string):
	for line in /\n/.Split(s):
		yield line
		
print join(lines("foo\nbar"))
