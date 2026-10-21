"""
BCE0149-6.boo(10,19): BCE0149: The type 'NotAThing' must derive from 'IThing' in order to substitute the generic parameter 'T' in 'Registry[of T]'.
"""
interface IThing:
	pass

class Registry[of T(IThing, class)]:
	pass

class BadRegistry(Registry of NotAThing):
	pass

class NotAThing:
	pass
