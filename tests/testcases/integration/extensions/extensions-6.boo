"""
ToUpper
BOO
"""
[Extension]
def ToUpper(s as string, beginIndex as int, endIndex as int):
	print "ToUpper"
	return s[beginIndex:endIndex].ToUpper()
	
print "zaboomba".ToUpper(2, 5)
