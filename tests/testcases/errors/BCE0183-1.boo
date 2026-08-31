"""
BCE0183-1.boo(7,5): BCE0183: Expression 'range' does not support slicing.
"""
# range is an overloaded method, not a keyword and not a local here, so there
# is no indexer to assign through.
def Write():
	range["start"] = 1
