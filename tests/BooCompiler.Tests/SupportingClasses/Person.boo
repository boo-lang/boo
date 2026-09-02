namespace BooCompiler.Tests.SupportingClasses

class Person:
	private _fname as string
	private _lname as string
	private _age as uint

	def constructor():
		pass

	public Age as uint:
		get: return _age
		set: _age = value

	public FirstName as string:
		get: return _fname
		set: _fname = value

	public LastName as string:
		get: return _lname
		set: _lname = value
