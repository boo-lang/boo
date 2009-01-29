"""
"""
import BooCompiler.Tests

class Base[of T(Base[of T])]:
	pass

class Internal(Base[of Internal]):
	pass

class External(GenericArgumentMustInheritSelf[of External]):
	pass

assert Internal() != null
assert External() != null

