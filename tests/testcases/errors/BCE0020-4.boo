"""
BCE0020-4.boo(11,9): BCE0020: An instance of type 'Foo' is required to access non static member 'go'.
BCE0020-4.boo(12,9): BCE0020: An instance of type 'Foo' is required to access non static member 'go2'.
"""
class Foo:
	def go [of T]():
		return
	def go2(param as int):
		return param
	static def fail():
	 	go[of int]()
	 	go2(0)

