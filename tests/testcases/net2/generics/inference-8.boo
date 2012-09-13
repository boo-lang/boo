"""
Int32
String
"""

import System

def Method[of TIn, TOut](arg1 as TIn, arg2 as callable(TIn) as TOut):
	print typeof(TIn).Name
	print typeof(TOut).Name
	
Method(42, { x as int | return x.ToString()})
