"""
BCW0011-9.boo(9,12): BCW0011: WARNING: Type 'Test' does not provide an implementation for 'ITest.Do()', a stub has been created.
"""

interface ITest:
	def Do() as string
	def Do[of T]() as T

class Test(ITest):
	def Do(x as string) as string:
		pass
	def Do[of T]() as T:
		pass

