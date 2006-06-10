"""
Hello Extension Methods
"""
[Extension]
def ToTitle(s as string):
	return join(word[:1].ToUpper() + word[1:] for word in s.Split(char(' ')))
	
print "hello extension methods".ToTitle()
