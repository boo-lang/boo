"""
, foo
foo, bar
"""
import System

class Observable:

	static _value = null

	static Value:
		get:
			return _value
		set:
			return if value == _value
			old = _value
			_value = value
			Changed(old, value)

	static event Changed as callable(object, object)

	
handler = { old, newValue | print "${old}, ${newValue}" }
Observable.Changed += handler

Observable.Value = "foo"
Observable.Value = "bar"

Observable.Changed -= handler

Observable.Value = "baz"



	
