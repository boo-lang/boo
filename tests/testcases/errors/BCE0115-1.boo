"""
BCE0115-1.boo(11,5): BCE0115: Cannot implement interface item 'ISpaceWaster.Property' when not implementing the interface 'ISpaceWaster'
"""

interface ISpaceWaster:
	Property as bool:
		get:
			pass

class SpaceSub:
	ISpaceWaster.Property as bool:
		get:
			return false
