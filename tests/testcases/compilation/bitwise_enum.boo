"""
Boo.Lang.Compiler.Tests.TestEnum
Foo, Bar
"""
import Boo.Lang.Compiler.Tests

a = TestEnum.Foo|TestEnum.Bar
print(a.GetType())
print(a)
