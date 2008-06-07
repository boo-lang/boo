"""
BCW0011-10.boo(9,12): BCW0011: WARNING: Type 'Test' does not provide an implementation for 'ITest.Do[of T]()', a stub has been created.
"""

interface ITest:
	def Do() as string
	def Do[of T]() as T

class Test(ITest):
	def Do() as string:
		pass
	def Do[of T](x as T) as T:
		pass

