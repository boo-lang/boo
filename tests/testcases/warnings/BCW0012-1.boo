"""
BCW0012-1.boo(8,7): BCW0012: WARNING: 'BooCompiler.Tests.SupportingClasses.ObsoleteClass.Bar' is obsolete. It is.
BCW0012-1.boo(9,1): BCW0012: WARNING: 'BooCompiler.Tests.SupportingClasses.ObsoleteClass.Foo()' is obsolete. Indeed it is.
BCW0012-1.boo(10,7): BCW0012: WARNING: 'BooCompiler.Tests.SupportingClasses.ObsoleteClass.Baz' is obsolete. We said so.
"""
import BooCompiler.Tests.SupportingClasses.ObsoleteClass from BooCompiler.Tests

print Bar
Foo()
print Baz


