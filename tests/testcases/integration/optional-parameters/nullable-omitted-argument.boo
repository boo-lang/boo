"""
1,none
1,2
3,none
3,4
"""
import BooCompiler.Tests.SupportingClasses from BooCompilerSupportingClasses

# An optional parameter that is also nullable, left out and supplied, on a
# static method and on a constructor. Constructors resolve on their own path,
# so a method-only test leaves that path uncovered.
print OptionalParameters.Nullable(1)
print OptionalParameters.Nullable(1, 2)
print OptionalNullableConstructor(3).Description
print OptionalNullableConstructor(3, 4).Description
