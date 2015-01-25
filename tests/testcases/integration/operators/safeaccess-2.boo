
c = 'string'
assert 'STRING' == c?.ToUpper()

d = {'foo': 'FOO'}
assert true == d['foo']?
assert false == d['bar']?

e = ('a', null, 'c')
assert true == e[0]?
assert false == e[1]?
assert 'ac' == e?[0] + e?[2]

