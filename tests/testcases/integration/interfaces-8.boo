"""
1
2
3
"""
import System.Collections

interface IEnumerable2(IEnumerable):
	pass

class Enumerable(IEnumerable2):

	def GetEnumerator() as IEnumerator:
		return (1, 2, 3).GetEnumerator()

def printall(foo as IEnumerable2):	
	for item in foo:
		print(item)

e = Enumerable()
printall(e)
	

