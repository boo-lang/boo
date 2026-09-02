"""
exact
default
"""
import BooCompiler.Tests.SupportingClasses from BooCompilerSupportingClasses

# Prefer(int) and Prefer(int, int = 2) are both applicable for one argument.
# This test tells us whether an exact match beats one needing a default filled.
print OptionalParameters.Prefer(1)
print OptionalParameters.Prefer(1, 2)
