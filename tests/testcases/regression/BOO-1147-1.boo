"""
"""
import BooCompiler.Tests.SupportingClasses from BooCompiler.Tests

class External(GenericArgumentMustInheritSelf[of External]):
	pass

class External2(GenericSelf[of External2]):
	pass

class External3(GenericSelf[of External3, int]):
	pass

assert External() != null
assert External2() != null
assert External3() != null

