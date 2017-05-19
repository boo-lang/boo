"""
1
4
"""
namespace TURBU.RM2K.Import

import System
import System.Linq.Enumerable

class Foo:
	[Getter(IsNew)]
	_new as bool
	
	[Getter(ID)]
	_id as int
	
	def constructor(id as int, isNew as bool):
		_id = id
		_new = isNew

static class Bar:
	def Process(value as Foo*) as int*:
		for cell in value.Where({c | c.IsNew}):
			yield cell.ID

for id in Bar.Process((Foo(1, true), Foo(2, false), Foo(3, false), Foo(4, true))):
	print id