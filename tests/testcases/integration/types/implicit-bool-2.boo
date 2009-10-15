"""
before
OverrideBoolOperator.operator bool
o is false
after
before
OverrideBoolOperator.operator bool
not o is true
after
"""
import BooCompiler.Tests.SupportingClasses from BooCompiler.Tests

o = ExtendsOverridenBoolOperator()
print "before"
if o:
	print "o is true"
else:
	print "o is false"
print "after"
print "before"
if not o:
	print "not o is true"
else:
	print "not o is false"
print "after"
