"""
BooCompiler.Tests.TestEnum
Foo, Bar
"""
import BooCompiler.Tests

a = TestEnum.Foo|TestEnum.Bar
print(a.GetType())
print(a)
