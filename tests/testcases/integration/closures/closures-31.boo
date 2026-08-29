"""
42
40
no return value
"""
// optional 'return' when there's a single expression
f = { i as int | i*2 }
print f(21)
assert f.GetType().GetMethod("Invoke").ReturnType is int

v = f(10) * 2
print v

g = { print "no return value" }
g()

assert g.GetType().GetMethod("Invoke").ReturnType is void
