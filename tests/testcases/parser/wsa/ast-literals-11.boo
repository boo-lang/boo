"""
p1 = [|
	Foo:
		get:
			return 'foo'
|]

p2 = [|
	Foo as string:
		get:
			return 'bar'
|]

p3 = [|
	Foo:
		set:
			_foo = value
|]

p4 = [|
	Foo as string:
		set:
			_foo = value
|]
"""
p1 = [|
	Foo:
		get:
			return 'foo'
		end
	end
|]

p2 = [|
	Foo as string:
		get:
			return 'bar'
		end
	end
|]

p3 = [|
	Foo:
		set:
			_foo = value
		end
	end
|]

p4 = [|
	Foo as string:
		set:
			_foo = value
		end
	end
|]

