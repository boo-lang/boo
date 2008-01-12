"""
BCE0153-1.boo(13,6): BCE0153: 'System.AttributeUsageAttribute' can be applied on one of these targets only : Class.
BCE0153-1.boo(7,2): BCE0153: 'System.AttributeUsageAttribute' can be applied on one of these targets only : Class.
"""
import System

[AttributeUsage(AttributeTargets.Method)]
def TestAttributeUsageCanOnlyBeAppliedToClass():
	pass

[AttributeUsage(AttributeTargets.Method)]
class Test:
	[AttributeUsage(AttributeTargets.Method)]
	[getter(MyProp)]
	_myProp = 0

