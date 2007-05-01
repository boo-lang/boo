"""
BCW0001-9.boo(13,12): BCW0011: WARNING: Type 'Ship' does not provide an implementation for 'GameObject.fireOn(single)', a stub has been created.
"""
class AlienShip(Ship):
	pass

interface IGameObject:
	def fireOn(energy as single)
	
abstract class GameObject(IGameObject):
	pass

class Ship(GameObject):
	pass
