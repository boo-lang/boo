"""
readonlyspan-refarg.boo(11,10): BCE0127: A ref or out argument must be an lvalue: '*span.get_Item(0)'
readonlyspan-refarg.boo(11,5): BCE0017: The best overload for the method 'Readonlyspan_refargModule.bump(char)' is not compatible with the argument list '(char)'.
"""
import System

def bump(ref c as char):
	c = char('X')

span = "Hi".AsSpan()
bump(span[0])
