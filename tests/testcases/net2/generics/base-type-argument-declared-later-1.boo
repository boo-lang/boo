"""
Registry of Thing
"""
interface IThing:
	pass

class Registry[of T(IThing, class)]:
	override def ToString():
		return "Registry of " + typeof(T).Name

# The constraint on T is checked while ThingRegistry's base type is bound, and
# Thing, which T is, is declared further down with its own base types unbound.
class ThingRegistry(Registry of Thing):
	pass

class Thing(Parent):
	pass

class Parent(IThing):
	pass

print ThingRegistry()
