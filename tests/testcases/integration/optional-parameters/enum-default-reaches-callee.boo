"""
3
a||b
"""
import System

# An enum default arrives as the enum rather than an integer, the shape that
# first broke resolution here. None is zero, so declared-defaults covers the
# non-zero case with Shade.Bright, not this one.
# This test tells us whether an enum-typed default resolves and reaches the callee.
parts = "a,,b".Split(char(','))
Console.WriteLine(parts.Length)
Console.WriteLine(join(parts, "|"))
