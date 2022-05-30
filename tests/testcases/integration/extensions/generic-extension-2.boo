"""
spam (System.String)
42 (System.Int32)
"""
import Boo.Lang.Compiler

[Extension]
def PrintWith[of T, U](left as T, right as U):
	print "${left} (${typeof(T)})"
	print "${right} (${typeof(U)})"
	
"spam".PrintWith(42)