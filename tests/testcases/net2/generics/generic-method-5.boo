"""
Foo3
Foo2
Foo1
"""

import System.Collections.Generic

class Foo:
	[property(Name)] _name as string

	public def constructor(name):
		_name = name

def MakeList[of T](item1 as T, item2 as T, item3 as T):
	list = List of T()
	list.Add(item3)
	list.Add(item2)
	list.Add(item1)
	return list

for foo in MakeList[of Foo](Foo("Foo1"), Foo("Foo2"), Foo("Foo3")):
	print foo.Name
