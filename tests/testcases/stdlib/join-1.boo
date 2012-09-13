assert "0 1 2" == join(range(3))
assert "0, 1, 2", join(range(3), ", ")
assert "0:1:2", join(range(3), ":")
assert "0:1:2", join(range(3), ":"[0])
