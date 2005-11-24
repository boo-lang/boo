"""
ToUpper
BOO
"""
def ToUpper(self as string, beginIndex as int, endIndex as int):
	print "ToUpper"
	return self[beginIndex:endIndex].ToUpper()
	
print "zaboomba".ToUpper(2, 5)
