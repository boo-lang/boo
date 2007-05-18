"""
test_implicit_B
test_implicit_int
test_overload(object)
method1(A)
"""

struct A:
	Val as int
	def constructor(v as int):
		Val = v
	def op_Implicit(inst as A) as B:
		return B(inst.Val)
	def op_Implicit(inst as A) as int:
		return inst.Val
		
struct B:
	Val as int
	def constructor(v as int):
		Val = v
	
def test_implicit_B(p as B):
	print "test_implicit_B"

def test_implicit_int(p as int):
	print "test_implicit_int"

//upcast favored over op_implicit
def test_overload(p as int):
	print "ERROR: test_overload(int)"

def test_overload(p as object):
	print "test_overload(object)"

//exact match still favored most of course
def method1(p as A):
	print "method1(A)"
	
def method1(p as B):
	print "method1(B)"


a = A(3)
test_implicit_B(a)
test_implicit_int(a)

test_overload(a)

method1(a)

