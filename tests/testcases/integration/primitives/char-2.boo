ch = cast(char, 97)
assert ch.GetType() is char
assert "a"[0] == ch
assert 97 == cast(int, ch)
assert cast(char, 65535) == char.MaxValue
assert cast(char, 0) == char.MinValue

