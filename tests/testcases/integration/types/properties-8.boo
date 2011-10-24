import System.Reflection

class Person:

	_fname = ""
	
	virtual FirstName as string:
		get:
			return _fname
		set:
			_fname = value
			
p = typeof(Person).GetProperty("FirstName")
getter = p.GetGetMethod()
setter = p.GetSetMethod()

assert getter is not null
assert getter.IsPublic, "getter.IsPublic"
assert getter.IsVirtual, "getter.IsVirtual"

assert setter is not null
assert setter.IsPublic, "setter.IsPublic"
assert setter.IsVirtual, "setter.IsVirtual"

