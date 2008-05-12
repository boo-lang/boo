"""
IInterface.Method
"""

interface IInterface:
	def Method()

class ExplicitImplementation(IInterface):
	def Method():
		print "Method"
	def IInterface.Method():
		print "IInterface.Method"

t as IInterface = ExplicitImplementation()
t.Method()

