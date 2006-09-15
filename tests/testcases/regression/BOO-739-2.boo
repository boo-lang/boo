class Foo:
	static a = 'hello'
	
	static b = a.ToUpper()

	static def constructor():
		assert a == 'hello'
		assert b == 'HELLO'

Foo()
