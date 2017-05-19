"""
Works!
"""
namespace TURBU.RM2K.Import

import System

def Invoke(value as Delegate):
	value.DynamicInvoke()

def Invoke(value as Delegate, count as int):
	value.DynamicInvoke()

Invoke() do:
	print "Works!"