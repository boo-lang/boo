namespace Boo.Lang.Tests.Environments

import Boo.Lang.Environments

class TestEnvironment(IEnvironment):
	_instance as object
	ProvideCount as int:
		get: return _provideCount

	_provideCount as int

	def constructor(instance as object):
		_instance = instance

	def Provide[of TNeed]() as TNeed:
		_provideCount += 1
		if _instance isa TNeed:
			return _instance
		raise System.InvalidOperationException()
