"""
Base
"""

import System

public class Base:
	pass

public class Derived (Base):
	pass

def Method[of T](arg1 as T, arg2 as T):
	print typeof(T).Name
	
Method(Base(), Derived())
