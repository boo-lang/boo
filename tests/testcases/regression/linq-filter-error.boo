"""
Emily
Jessica
John
William
"""
namespace linq_filter_error

import System
import System.Collections.Generic
import System.Linq.Enumerable

class Bar:
	[Getter(ID)]
	_id as int
	
	[Getter(Name)]
	_name as string
	
	def constructor(id as int, name as string):
		_id = id
		_name = name

class Foo[of T(Bar)]:

	private _data as T*
	private _dict = Dictionary[of int, T]()
	
	def constructor(data as T*):
		_data = data

	public def Load():
		for value in _data.Where({v | not _dict.ContainsKey(v.ID)}):
			_dict.Add(value.ID, value)
	
	public def Print():
		for value in _dict.Values.Select({t | t.Name}).OrderBy({n|return n}):
			print value


var names = (Bar(1, "John"), Bar(2, "William"), Bar(3, "Jessica"), Bar(2, "Rachael"), Bar(3, "Fred"), Bar(4, "Emily"))
var list = Foo[of Bar](names)
list.Load()
list.Print()