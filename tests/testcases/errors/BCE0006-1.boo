"""
BCE0006-1.boo(5,5): BCE0006: 'int' is a value type. The 'as' operator can only be used with reference types.
BCE0006-1.boo(6,17): BCE0006: 'long' is a value type. The 'as' operator can only be used with reference types.
"""
a = 3 as object
b = object() as long


