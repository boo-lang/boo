"""
Derived
Boolean
"""

import System

public class Program:
	
	def Method[of TIn, TOut](arg1 as TIn, arg2 as Converter[of TIn, TOut]):
		print typeof(TIn).Name
		print typeof(TOut).Name
	
class Base:
	pass
	
class Derived (Base):
	pass


Program().Method(Derived(), { x as Base | return true })
