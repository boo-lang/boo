"""
it wasn't a string.
A STRING
"""
import System

def use(obj):
	try:
		print(cast(string, obj).ToUpper())
	except x as InvalidCastException:
		print("it wasn't a string.")

use(1)
use("a string")
