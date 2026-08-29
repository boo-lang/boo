"""
collection: foo bar
enumerable: 0 1 2
object: baz bag
"""
import System.Collections

def use(obj):
	print("object: " + join(obj))

def use(collection as ICollection):
	print("collection: " + join(collection))

def use(enumerable as IEnumerable):
	print("enumerable: " + join(enumerable))

use(["foo", "bar"])
use(range(3))
use(cast(object, ["baz", "bag"]))
