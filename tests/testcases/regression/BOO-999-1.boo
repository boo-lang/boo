"""
foo-B
"""

class A:
        pass
class B(A):
        pass
class C(B):
        pass

class Test:
    def foo(x as A):
        print "foo-A"

    def foo(x as B):
        print "foo-B"

c = C()

test as duck = Test()
test.foo(c)

