"""
BCW0001-10.boo(9,18): BCW0001: WARNING: Type 'GameObject' does not provide an implementation for 'IGameObject.Fire' and will be marked abstract.
"""
import System

interface IGameObject:
	event Fire as EventHandler
	
class GameObject(IGameObject):
	pass

