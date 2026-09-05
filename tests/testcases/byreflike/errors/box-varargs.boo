"""
box-varargs.boo(10,6): BCE0190: Cannot box byref-like type 'System.ReadOnlySpan[of char]': it has no conversion to 'object'.
"""
import System

def show(*args as (object)):
	Console.WriteLine(args.Length)

span = "Hello".AsSpan()
show(span)
