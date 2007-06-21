"""
BCE0114-1.boo(12,5): BCE0114: Explicit interface implementation for non interface type 'SpaceWaster'
"""

class SpaceWaster:
	Property as bool:
		get:
			return true

class SpaceSub (SpaceWaster):
	
	SpaceWaster.Property as bool:
		get:
			return false
