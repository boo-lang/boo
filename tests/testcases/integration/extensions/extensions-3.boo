"""
IEnumerable.Each
Hello
interface
extensions
"""
[Extension]
def Each(e as System.Collections.IEnumerable, action as callable(object)):
	print "IEnumerable.Each"
	for item in e:
		action(item)
		
["Hello", "interface", "extensions"].Each(print)
