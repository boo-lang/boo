"""
BCW0011-15.boo(6,16): BCW0011: WARNING: Type 'Consumer' does not provide an implementation for 'BooCompiler.Tests.SupportingClasses.BaseInterface.Add(string)', a stub has been created.
"""
import BooCompiler.Tests.SupportingClasses from BooCompiler.Tests
		
class Consumer(BaseInterface, BaseAbstractClassWithoutImplementation):
	pass
