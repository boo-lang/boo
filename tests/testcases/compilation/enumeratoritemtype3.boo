"""
2
"""
class Foo:
	def GetEnumerator[of T]() as System.Collections.IEnumerator: #generic method GetEnumerator must be ignored
		pass
	def GetEnumerator(_ as int) as int: #ignored GetEnumerator with parameter(s)
		pass
	def GetEnumerator() as System.Collections.Generic.IEnumerator[of int]: #this one will be selected
		yield 1

for i in Foo():
	print(i*2) # i is declared as int
