"""
03
"""

callable GenericDelegate[of TIn, TOut](argument as TIn) as TOut

a as GenericDelegate[of int, string] = { i as int | i.ToString("00") }
print a(3)


