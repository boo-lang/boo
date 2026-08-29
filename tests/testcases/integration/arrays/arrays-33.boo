"""
1 2 3 4
"""
t = matrix(int, 2, 2)

t.SetValue(1, 0, 0)
t.SetValue(2, 0, 1)
t.SetValue(3, 1, 0)
t.SetValue(4, 1, 1)

print join(t)
