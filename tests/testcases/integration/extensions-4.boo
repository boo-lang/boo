"""
List.Each
Hello
interface
extensions
IEnumerable.Each
1
2
3
"""
def Each(self as System.Collections.IEnumerable, action as callable(object)):
	print "IEnumerable.Each"
	for item in self:
		action(item)
		
def Each(self as List, action as callable(object)):
	print "List.Each"
	for i in range(len(self)):
		action(self[i])
		
["Hello", "interface", "extensions"].Each(print)
(1, 2, 3).Each(print)
