"""
BCE0045-1.boo(9,1): BCE0045: Macro expansion error: Usage: `macro [<parent.>+]<name>[(arg0,...)]`.
BCE0045-1.boo(12,1): BCE0045: Macro expansion error: Usage: `macro [<parent.>+]<name>[(arg0,...)]`.
BCE0045-1.boo(16,5): BCE0045: Macro expansion error: Nested macro extension cannot be itself a nested macro.
BCE0045-1.boo(19,1): BCE0045: Macro expansion error: No macro `nonexisting` has been found to extend.
BCE0045-1.boo(22,1): BCE0045: Macro expansion error: No macro `base.nonexisting` has been found to extend.
BCE0045-1.boo(25,1): BCE0045: Macro expansion error: Extending macro `macro` is not supported.
"""
macro:
	pass

macro "foo":
	pass

macro base:
	macro base.nested:
		pass

macro nonexisting.nested:
	pass

macro base.nonexisting.nested:
	pass

macro macro.foo:
	pass

