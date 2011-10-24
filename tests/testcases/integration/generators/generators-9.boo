

a = (i*5 for i in range(3), i*2 for i in range(3))

# assert typeof((AbstractGenerator)) is a.GetType()
assert "0, 5, 10", join(a[0], " == ")
assert "0, 2, 4", join(a[1], " == ")
