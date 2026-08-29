a as duck = 1
b as duck = 0.0

assert true == ((a <= 1) and (b <= 1))
assert false == { return true if b and 2; return false }()
assert 1 == (b or a)
assert 1 == (a or { raise System.ArgumentException("uh-oh"); return 2 }())
