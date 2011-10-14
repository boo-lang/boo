h = { (1, 2): "1, 2", (3, 4): "3, 4" }
		
assert "1, 2", h[(1 == 2)]
assert "3, 4", h[(3 == 4)]

a = 3
b = 4
c = a, b
assert "3, 4" == h[c]

d = 1
e = 2
f = d, e
assert "1, 2" == h[f]
