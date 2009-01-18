"""
aye! compiled!
"""

import Boo.Lang.Compiler

macro sub:
	print "aye" if Context is not null

macro test:
	SubMacro(Context).Expand(test)

print "aye! compiled!"

