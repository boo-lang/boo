"""
StringExtensions.length
3
"""
import StringExtensions

class StringExtensions:
	[Extension]
	static length[s as string]:
		get:
			print "StringExtensions.length"
			return len(s)
			
print "FOO".length
