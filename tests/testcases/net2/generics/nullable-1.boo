import System

d = Nullable of date()
assert not d.HasValue

value = date.Now
d = Nullable[of date](value)
assert d.HasValue
assert d.Value == value

d2 as date? = value
assert d2.Value == value

