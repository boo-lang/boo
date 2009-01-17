"""
Int32
String
0042
"""

import System

def Method[of TIn, TOut](item as TIn, converter as Converter[of TIn, TOut]):
	print typeof(TIn).Name
	print typeof(TOut).Name
	print converter(item)
	
Method(42, { x | return x.ToString("0000")})
