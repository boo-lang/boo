"""
Int32
String
"""

import System

def Method[of TIn, TOut](arg1 as TIn, arg2 as Converter[of TIn, TOut]):
	print typeof(TIn).Name
	print typeof(TOut).Name
	
Method(42, { x as int | return x.ToString()})
