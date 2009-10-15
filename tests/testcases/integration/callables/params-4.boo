"""
VarArgs.Method(1, 2)
VarArgs.Method(System.Object[])
VarArgs.Method
"""
import BooCompiler.Tests.SupportingClasses from BooCompiler.Tests

d = VarArgs()
d.Method(1, 2)
d.Method((object(),))
d.Method()
