"""
Hello Extension Methods
"""
def ToTitle(self as string):
	return join(word[:1].ToUpper() + word[1:] for word in self.Split(char(' ')))
	
print "hello extension methods".ToTitle()
