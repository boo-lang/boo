"""
collection: foo bar
enumerable: 0 1 2
"""
import System.Collections

def use(collection as ICollection):
	print("collection: " + join(collection))

def use(enumerable as IEnumerable):
	print("enumerable: " + join(enumerable))

use(["foo", "bar"])
use(range(3))
