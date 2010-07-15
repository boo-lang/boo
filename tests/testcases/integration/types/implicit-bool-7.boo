"""
before
OverrideBoolOperator.operator bool
o is false
after
"""
import BooCompiler.Tests.SupportingClasses from BooCompiler.Tests

class Component:
	o:
		get: return ExtendsOverridenBoolOperator()
		
	def run():
		print "before"
		if o and o is not null:
			print "o is true"
		else:
			print "o is false"
		print "after"
		
Component().run()
