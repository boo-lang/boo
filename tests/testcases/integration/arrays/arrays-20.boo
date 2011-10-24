

a = array(range(3))
assert a.GetType() is System.Type.GetType("System.Int32[]")

assert 1 == a[-1]-a[-2]
assert -1 == a[-2]-a[-1]
