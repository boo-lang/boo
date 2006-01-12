"""
before
ValueTypeOverrideBoolOperator.operator bool
o is false
after
before
ValueTypeOverrideBoolOperator.operator bool
not o is true
after
"""
import BooCompiler.Tests

o as duck = ValueTypeOverrideBoolOperator()
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
