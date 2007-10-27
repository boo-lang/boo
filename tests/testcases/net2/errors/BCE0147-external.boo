"""
BCE0147-external.boo(5,33): BCE0147: The type 'System.Type' must be a value type in order to substitute the generic parameter 'System.Nullable`1.T'.
"""

print typeof(System.Nullable[of System.Type]).Name
