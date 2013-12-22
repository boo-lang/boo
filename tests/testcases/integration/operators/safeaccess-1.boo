class Foo:
	public field as Foo
	property prop as Foo
	self[s as Foo] as Foo:
		get: return s
	def foo(result) as Foo:
		return result

f as Foo

assert null == f?.field
assert null == f?.prop
assert null == f?[null]
assert null == f?.foo(null)

f = Foo()
f.field = f
f.prop = f

assert f == f?.field
assert f == f?.prop
assert f == f?[f]
assert f == f?.foo(f)

f = Foo()

assert null == f?.field?.prop
assert null == f?.prop?.field
assert null == f?[null]?.foo(f)
assert null == f?.foo(null)?[f]

