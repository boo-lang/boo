"""
1,9
5,2
False,q,42,9000000000,1.5,hi,Bright
"""
import BooCompiler.Tests.SupportingClasses from BooCompilerSupportingClasses

# Some arguments written out, the rest left to their defaults.
# This test tells us whether the split between the two lands in the right place.
print OptionalParameters.Trailing(1, 9)
print OptionalParameters.AllOptional(5)
print OptionalParameters.Shapes(false, char('q'))
