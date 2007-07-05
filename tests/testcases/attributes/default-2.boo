"""
class Test:

	Foo as string:
		set:
			if value is null:
				value = 'test'
			_foo = value

	_foo as string
"""
class Test:
	[default("test")]
	Foo as string:
		set:
			_foo = value
	_foo as string
