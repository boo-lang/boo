"""
42
"""

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast

macro setDefaultFieldVisibilityToPublic:
	Context.Parameters.DefaultFieldVisibility = TypeMemberModifiers.Public

class ProtectedNowPublicField:
	foo = 42

setDefaultFieldVisibilityToPublic
print ProtectedNowPublicField().foo

