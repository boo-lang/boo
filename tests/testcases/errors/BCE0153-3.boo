"""
BCE0153-3.boo(6,2): BCE0153: 'System.AttributeUsageAttribute' can be applied on one of these targets only : Class.
"""
import System

[AttributeUsage(AttributeTargets.All)]
struct AStructIsNotAClass:
	public a as int
