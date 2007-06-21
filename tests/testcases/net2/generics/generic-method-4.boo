"""
I
Really
Hope
This
Works
"""
import System.Collections.Generic

def ArrayToList[of T](items as (T)):
	list = List of T()
	for item in items: list.Add(item)
	return list

for s in ArrayToList[of string](("I", "Really", "Hope", "This", "Works")):
	print s

