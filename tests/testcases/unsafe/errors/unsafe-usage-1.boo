"""
unsafe-usage-1.boo(8,1): BCE0045: Macro expansion error: Usage: `unsafe [<ptrName> as <ptrType> = <data>]+'.
unsafe-usage-1.boo(10,1): BCE0045: Macro expansion error: Usage: `unsafe [<ptrName> as <ptrType> = <data>]+'.
unsafe-usage-1.boo(12,1): BCE0045: Macro expansion error: Usage: `unsafe [<ptrName> as <ptrType> = <data>]+'.
unsafe-usage-1.boo(14,1): BCE0045: Macro expansion error: `unsafe` is useless without a body.
"""
bytes = array[of byte](1)
unsafe:
	pass
unsafe bp:
	pass
unsafe bp = bytes:
	bp++
unsafe bp as byte = bytes

