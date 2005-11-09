"""
IEnumerable.Each
Hello
interface
extensions
"""
def Each(self as System.Collections.IEnumerable, action as callable(object)):
	print "IEnumerable.Each"
	for item in self:
		action(item)
		
["Hello", "interface", "extensions"].Each(print)
