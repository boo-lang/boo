"""
1,2
1,2
True,z,42,9000000000,1.5,hi,Bright
<null>,<null>
False
"""
import BooCompiler.Tests.SupportingClasses from BooCompilerSupportingClasses

# Every shape a default can take, all left out at once: bool, char, int, long,
# double, string, a non-zero enum, null for a reference type, and default(T)
# for a struct, which arrives as null rather than a value.
# This test tells us whether the compiler can turn each shape into a literal.
print OptionalParameters.Trailing(1)
print OptionalParameters.AllOptional()
print OptionalParameters.Shapes()
print OptionalParameters.Nulls()
print OptionalParameters.Token()
