"""
invalid-array-slice.boo(6,9): BCE0022: Cannot convert 'bool' to 'int'.
invalid-array-slice.boo(6,15): BCE0022: Cannot convert 'bool' to 'int'.
"""
a = (1, 2, 3)
print a[false:true]
