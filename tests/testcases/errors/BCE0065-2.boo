"""
BCE0065-2.boo(27,13): BCE0065: Cannot iterate over expression of type 'Bar'.
BCE0065-2.boo(29,13): BCE0065: Cannot iterate over expression of type 'Baz'.
"""


class Foo:
	def GetEnumerator[of T]() as System.Collections.IEnumerator: #generic method GetEnumerator must be ignored
		pass
	def GetEnumerator(_ as int) as int: #ignored GetEnumerator with parameter(s)
		pass
	def GetEnumerator() as System.Collections.Generic.IEnumerator[of int]: #this one will be selected
		yield 1

class Bar:
	def GetEnumerator() as int:
		return 0

class Baz:
	def GetEnumerator[of T]() as System.Collections.Generic.IEnumerator[of int]:
		yield 1



for i in Foo(): #can iterate Foo even if it does not implement IEnumerable
	print i
for i in Bar(): #!
	print i
for i in Baz(): #!
	print i

