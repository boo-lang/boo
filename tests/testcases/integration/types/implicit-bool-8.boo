"""
before
OverrideBoolOperator.operator bool
OverrideBoolOperator.operator bool
o is false
after
"""
import BooCompiler.Tests.SupportingClasses from BooCompiler.Tests

class Component:
	o:
		get: return ExtendsOverridenBoolOperator()
		
	def run():
		return o and invalid()
		
	def invalid():
		print "invalid"
		return true
		
print "before"
if Component().run(): # o is returned by run and the implicit operator is evaluated again
	print "o is true"
else:
	print "o is false"
print "after"
