"""
"""
macro impc:
	for a in impc.Arguments:
		yield [| import System.Collections($a) |]
	
impc IEnumerable, IEnumerator

def iterator(e as IEnumerable) as IEnumerator:
	pass
	
assert iterator([]) is null
