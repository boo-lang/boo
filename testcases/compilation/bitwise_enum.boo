"""
Boo.Tests.Lang.Compiler.TestEnum
Foo, Bar
"""
import Boo.Tests.Lang.Compiler

a = TestEnum.Foo|TestEnum.Bar
print(a.GetType())
print(a)
