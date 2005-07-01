"""
42
32
"""
callable handler(a as int) as int

class Action:
	cb as handler
	def constructor(callback as handler):
		cb = callback
	def Execute():
		cb(0)

class A:
	a as Action
	[getter(Go)]
	ac = Action() def(para):
		newa = A(32)
	def constructor(p):
		a = ac
		print p

A(42).Go.Execute()
