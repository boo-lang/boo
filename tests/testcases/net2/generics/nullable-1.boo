import System

d = Nullable of date()
assert not d.HasValue

value = date.Now
d = Nullable[of date](value)
assert d.HasValue
assert d.Value == value
