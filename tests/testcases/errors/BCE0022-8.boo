"""
BCE0022-8.boo(5,18): BCE0022: Cannot convert '(System.Int32)' to '(System.Int32, 2)'.
BCE0022-8.boo(6,14): BCE0022: Cannot convert '(System.Int32, 2)' to '(System.Int32)'.
"""
a as (int, 2) = (1, 2)
b as (int) = a
