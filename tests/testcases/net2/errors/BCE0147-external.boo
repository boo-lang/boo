"""
BCE0147-external.boo(6,38): BCE0147: The type 'T' must be a value type in order to substitute the generic parameter 'T' in 'System.Nullable[of T]'. 
BCE0147-external.boo(12,14): BCE0147: The type 'System.Type' must be a value type in order to substitute the generic parameter 'T' in 'System.Nullable[of T]'. 
"""

def NullableArgBad[of T](argument as T?): #!
	pass

def NullableArgGood[of T(struct)](argument as T?):
	pass

print typeof(System.Nullable[of System.Type]).Name #!
