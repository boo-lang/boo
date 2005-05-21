"""
BCE0116-1.boo(11,12): BCE0116: Explicit member implementation for 'ISpaceWaster.Property' must not declare any modifiers.
"""

interface ISpaceWaster:
	Property as bool:
		get:
			pass

class SpaceSub(ISpaceWaster):
	public ISpaceWaster.Property as bool:
		get:
			return false
