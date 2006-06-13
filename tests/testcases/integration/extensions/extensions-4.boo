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
[Extension]
def Each(e as System.Collections.IEnumerable, action as callable(object)):
	print "IEnumerable.Each"
	for item in e:
		action(item)
		
[Extension]
def Each(l as List, action as callable(object)):
	print "List.Each"
	for i in range(len(l)):
		action(l[i])
		
["Hello", "interface", "extensions"].Each(print)
(1, 2, 3).Each(print)
