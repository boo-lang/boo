#FIXME: replaced -1s with 42 temporarily
#       parenthesis surround -1 now that enum initializer is an Expression (as of rev.3228)
"""
enum AnEnum:

	Foo

	Bar = 1

	Baz = 42

enum AnotherEnum:

	Foo = 42

enum YetAnother:

	Foo

	Bar

enum Empty:
	pass
"""
enum AnEnum:
	Foo
	Bar = 1
	Baz = 42
	
enum AnotherEnum:
	Foo = 42
	
enum YetAnother:
	Foo
	Bar

enum Empty:
	pass

