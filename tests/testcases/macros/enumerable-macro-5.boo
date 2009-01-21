"""
Good news everyone!
"""
import Boo.Lang.Compiler.MetaProgramming


macro yieldNot:
	pass

macro yieldBlock:
	yield


yieldNot:
	print "Where am I now?"

yieldBlock:
	print "Good news everyone!"

