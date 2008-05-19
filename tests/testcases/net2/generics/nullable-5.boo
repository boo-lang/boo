a = 0
c = 1
na as int? = 0
nb as int? = 0
nc as int? = 1
nn as int?
no as int?

assert na != nn
assert na = a
assert na != c
assert na == nb
assert na != nc
assert nn == no

assert na > nn
assert nn < na
assert c > na
assert nc > a
assert na < c
assert nn < a
assert nn < c

assert na >= nb
assert c >= nc

