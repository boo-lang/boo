import System.Reflection

class Person:

	_fname = ""
	
	FirstName as string:
		get:
			return _fname
		virtual set:
			_fname = value
			
p = typeof(Person).GetProperty("FirstName")
getter = p.GetGetMethod()
setter = p.GetSetMethod()

assert getter is not null
assert getter.IsPublic, "getter.IsPublic"
assert not getter.IsVirtual, "not getter.IsVirtual"

assert setter is not null
assert setter.IsPublic, "setter.IsPublic"
assert setter.IsVirtual, "setter.IsVirtual"

