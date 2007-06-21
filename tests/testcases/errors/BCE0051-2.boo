"""
BCE0051-2.boo(13,20): BCE0051: Operator '|' cannot be used with a left hand side of type 'BooCompiler.Tests.TestEnum' and a right hand side of type 'int'.
BCE0051-2.boo(14,9): BCE0051: Operator '|' cannot be used with a left hand side of type 'int' and a right hand side of type 'BooCompiler.Tests.TestEnum'.
BCE0051-2.boo(15,20): BCE0051: Operator '&' cannot be used with a left hand side of type 'BooCompiler.Tests.TestEnum' and a right hand side of type 'int'.
BCE0051-2.boo(16,9): BCE0051: Operator '&' cannot be used with a left hand side of type 'int' and a right hand side of type 'BooCompiler.Tests.TestEnum'.
BCE0051-2.boo(17,20): BCE0051: Operator '^' cannot be used with a left hand side of type 'BooCompiler.Tests.TestEnum' and a right hand side of type 'int'.
BCE0051-2.boo(18,9): BCE0051: Operator '^' cannot be used with a left hand side of type 'int' and a right hand side of type 'BooCompiler.Tests.TestEnum'.
BCE0051-2.boo(19,20): BCE0051: Operator '+' cannot be used with a left hand side of type 'BooCompiler.Tests.TestEnum' and a right hand side of type 'BooCompiler.Tests.TestEnum'.
BCE0051-2.boo(20,9): BCE0051: Operator '-' cannot be used with a left hand side of type 'int' and a right hand side of type 'BooCompiler.Tests.TestEnum'.
"""
import BooCompiler.Tests

print TestEnum.Bar | 1
print 1 | TestEnum.Bar
print TestEnum.Bar & 2
print 2 & TestEnum.Bar
print TestEnum.Bar ^ 3
print 3 ^ TestEnum.Bar
print TestEnum.Foo + TestEnum.Bar
print 3 - TestEnum.Bar
