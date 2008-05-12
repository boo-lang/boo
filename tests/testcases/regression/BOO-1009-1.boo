interface IInterface:
	Property as bool:
		get

class ExplicitImplementation(IInterface):
	Property:
		get:
			return false
	IInterface.Property:
		get:
			return true


t as IInterface = ExplicitImplementation()
assert t.Property

