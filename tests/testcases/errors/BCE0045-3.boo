"""
BCE0045-3.boo(13,1): BCE0045: Macro expansion error: `foo` macro invocation argument(s) did not match definition: `foo((x as string))`.
BCE0045-3.boo(17,1): BCE0045: Macro expansion error: `bar` macro invocation argument(s) did not match definition: `bar((x as bool))`.
"""
import Boo.Lang.PatternMatching

macro foo(x as string):
	pass

macro bar(x as bool):
	pass

foo 31
foo "ok"

bar true
bar 1

