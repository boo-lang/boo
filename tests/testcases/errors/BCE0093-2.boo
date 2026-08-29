"""
BCE0093-2.boo(5,10): BCE0093: Cannot branch into ensure block.
"""
try:
	goto label
ensure:
	:label

