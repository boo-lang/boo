"""
BCE0017-3.boo(7,19): BCE0127: A ref or out argument must be an lvalue: '1'
BCE0017-3.boo(7,15): BCE0017: The best overload for the method 'BooCompiler.Tests.ByRef.SetValue(int, ref int)' is not compatible with the argument list '(int, int)'.
"""
import BooCompiler.Tests

ByRef.SetValue(1, 1)
