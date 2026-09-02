"""
caught
"""
import System

# The filled default must be appended, not inserted: were paramName to take the
# first position, the supplied argument would be displaced and nothing thrown.
# This test tells us whether the argument the caller wrote keeps its place.
o as string = null
try:
	ArgumentNullException.ThrowIfNull(o)
	Console.WriteLine("not reached")
except x as ArgumentNullException:
	Console.WriteLine("caught")
