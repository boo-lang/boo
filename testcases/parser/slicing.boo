"""
l = [1, 2, 3]
assert((1 == l[0]))
assert((2 == l[1]))
assert(([1, 2] == l[:2]))
assert(([1] == l[0:1]))
assert(([1] == l[:1]))
assert((3 == l[(-1)]))
assert(([2, 3] == l[1:]))
assert(([3, 2, 1] == l[::(-1)]))
assert(([1, 2, 3] == l[:]))
assert(([1, 3] == l[::2]))

"""
l = [1, 2, 3]
assert( 1 == l[0] )
assert( 2 == l[1] )
assert( [1, 2] == l[:2] )
assert( [1] == l[0:1] )
assert( [1] == l[:1] )
assert( 3 == l[-1] )
assert( [2, 3] == l[1:] )
assert( [3, 2, 1] == l[::-1] )
assert( [1, 2, 3] == l[:] )
assert( [1, 3] == l[::2] )
