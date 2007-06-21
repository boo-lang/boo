"""
BCE0093-1.boo(4,6): BCE0093: Cannot branch into ensure block.
"""
goto label
try:
	pass
ensure:
	:label

