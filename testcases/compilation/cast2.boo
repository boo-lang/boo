"""
it wasn't a string
a string
"""
import System

def use(obj):
	try:
		print(cast(string, obj))
	except x as InvalidCastException:
		print("it wasn't a string.")

use(1)
use("a string")
