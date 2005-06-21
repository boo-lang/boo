"""
BCW0001-9.boo(14,12): BCW0001: WARNING: Type 'Ship' does not provide an implementation for 'GameObject.fireOn' and will be marked abstract
BCW0001-9.boo(5,17): BCW0001: WARNING: Type 'AlienShip' does not provide an implementation for 'Ship.fireOn' and will be marked abstract
"""
class AlienShip(Ship):
	pass

interface IGameObject:
	def fireOn(energy as single)
	
abstract class GameObject(IGameObject):
	pass
	
class Ship(GameObject):
	pass
	

