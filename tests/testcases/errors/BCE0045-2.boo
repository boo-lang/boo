"""
BCE0045-2.boo(11,5): BCE0045: Macro expansion error: Invalid nested macro context. Check your macro hierarchy.
BCE0045-2.boo(12,1): BCE0045: Macro expansion error: Invalid nested macro context. Check your macro hierarchy.
"""
import BooSupportingClasses.NestedMacros

macro foo2.a:
	yield

foo:
	a
a

