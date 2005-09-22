"""
before
GetEnumerator
1
2
after
"""
import System.Collections

class Enumerable(IEnumerable):
	def GetEnumerator():
		print "GetEnumerator"
		yield 1
		yield 2
		
print "before"
for item in Enumerable():
	print item
print "after"
