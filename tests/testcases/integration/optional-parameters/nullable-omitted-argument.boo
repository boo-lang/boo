"""
1,none
1,2
3,none
3,4
"""
import BooCompiler.Tests.SupportingClasses from BooCompilerSupportingClasses

# A parameter that is both optional and nullable was read from the arguments
# the caller wrote, which for an omitted one is off the end of them. A
# constructor resolves before the defaults are filled in, so it went first.
# This test tells us whether omitting such a parameter still resolves the call.
print OptionalParameters.Nullable(1)
print OptionalParameters.Nullable(1, 2)
print OptionalNullableConstructor(3).Description
print OptionalNullableConstructor(3, 4).Description
