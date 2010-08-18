"""
BCE0022-15-strict.boo(5,15): BCE0022: Cannot convert 'object' to 'string'.
"""
a = object()
s as string = a
s = a cast string # casting is ok
s = a as string # casting is ok

d as duck = object() # casting from duck is allowed
s = d
s = d cast string
s = d as string

[assembly: StrictMode]
