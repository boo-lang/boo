"""
BCW0012-1.boo(8,7): BCW0012: WARNING: 'BooCompiler.Tests.ObsoleteClass.Bar' is obsolete. It is.
BCW0012-1.boo(9,1): BCW0012: WARNING: 'BooCompiler.Tests.ObsoleteClass.Foo()' is obsolete. Indeed it is.
BCW0012-1.boo(10,7): BCW0012: WARNING: 'BooCompiler.Tests.ObsoleteClass.Baz' is obsolete. We said so.
"""
import BooCompiler.Tests.ObsoleteClass

print Bar
Foo()
print Baz


