namespace BooSupportingClasses

class Setters:

	ProtectedSet as bool:
		get:
			return true
		protected set:
			pass

	[property(MacroProtectedSet, ProtectedSetter:true)]
	_macroProtectedSet = 1

