"""
lhs is null
True
rhs is null
True
rhs is null
False
lhs is null
False
"""
import BooCompiler.Tests.SupportingClasses from BooCompiler.Tests

print null == OverrideEqualityOperators()
print OverrideEqualityOperators() == null

print OverrideEqualityOperators() != null
print null != OverrideEqualityOperators()

assert OverrideEqualityOperators() is not null
a = OverrideEqualityOperators()
b = OverrideEqualityOperators()
assert a is not b
assert b is not a
assert a is a
assert b is b


