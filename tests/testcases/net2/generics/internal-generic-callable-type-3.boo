"""
03
"""

callable GenericDelegate[of TIn, TOut](argument as TIn) as TOut

def GenericMethod[of TIn, TOut](arg as TIn, func as GenericDelegate[of TIn, TOut]):
	return func(arg)

print GenericMethod[of int, string](3, { i as int | i.ToString("00") })
