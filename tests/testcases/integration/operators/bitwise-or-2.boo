"""
BooCompiler.Tests.SupportingClasses.TestEnum
Foo, Bar
"""
import BooCompiler.Tests.SupportingClasses from BooCompiler.Tests

a = TestEnum.Foo|TestEnum.Bar
print(a.GetType())
print(a)
