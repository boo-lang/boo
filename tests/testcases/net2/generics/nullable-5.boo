a = 0
c = 1
na as int? = 0
nb as int? = 0
nc as int? = 1
nn as int?
no as int? = null

assert na != nn
assert not (na == nn)
assert na == a
assert not (na != a)
assert na != c
assert not (na == c)
assert na == nb
assert not (na != nb)
assert na != nc
assert not (na == nc)
assert nn == no
assert not (nn != no)

assert c > na
assert not (c < na)
assert nc > a
assert not (nc < a)
assert na < c
assert not (na > c)

assert na >= nb
assert na <= nb
assert not (na > nb)
assert not (na < na)
assert c >= nc
assert c <= nc
assert not (c > nc)
assert not (c < nc)
assert c >= na
assert not (c <= na)

#below ops with one or two non-value are false/undefined as in C#
assert not (na > nn)
assert not (na < nn)
assert not (nn < na)
assert not (nn > na)
assert not (nn < a)
assert not (nn > a)
assert not (nn < c)
assert not (nn > c)

assert not (nn >= a)
assert not (a <= nn)
assert not (nn <= c)
assert not (c <= nn)

assert not (nn < nn)
assert not (nn > nn)
assert not (nn >= nn)
assert not (nn <= nn)

