a = "spam", 42, object()

s as string, i as int, o = a
assert "spam" == s
assert 42 == i
assert o is a[-1]
