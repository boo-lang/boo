"""
BCE0147-external.boo(5,14): BCE0147: The type 'System.Type' must be a value type in order to substitute the generic parameter 'T' in 'System.Nullable[of T]'.
"""

print typeof(System.Nullable[of System.Type]).Name
