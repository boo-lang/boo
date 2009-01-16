"""
BCW0019-1.boo(8,1): BCW0019: WARNING: 'except ArgumentException:' is ambiguous. Did you mean 'except as ArgumentException:'?
"""
import System

try:
	int.Parse("forty two")
except ArgumentException:
	pass #actually a FormatException will be handled here"

