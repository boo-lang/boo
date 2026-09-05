"""
42
7
7
5
7
"""
import BooCompiler.Tests.SupportingClasses

r = RefReturn()

r.Item(0) = 42
print r.ReadItem(0)

r.Value = 7
print r.Read()
print r.Value

ByRef.SetValue(5, r.Item(1))
print r.ReadItem(1)
print r.ReadOnlyValue
