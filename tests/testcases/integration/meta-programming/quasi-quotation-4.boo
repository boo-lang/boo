"""
block :
	a = b if (a is not null)
"""
import Boo.Lang.Compiler

code = [|
	block:
		a = b if a is not null
|]
print code.ToCodeString()

