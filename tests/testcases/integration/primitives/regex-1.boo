s = "OK\nfoo\nbar"

assert false == /ok/.IsMatch(s)
assert false == /ok/gcne.IsMatch(s)

assert true == /ok/i.IsMatch(s)
assert true == /^ok/i.IsMatch(s)

assert false == /^foo/.IsMatch(s)
assert true == /^foo/m.IsMatch(s)

assert false == /^FOO/.IsMatch(s)
assert true == /^FOO/mi.IsMatch(s)

assert false == /foo.bar/.IsMatch(s)
assert true == /foo.bar/s.IsMatch(s)
