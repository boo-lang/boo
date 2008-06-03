"""
pre
post
1
pre
42
"""

namespace Boo1040.Testcase

class A:
	_x = "pre"

	def constructor():
		print _x

	def constructor(x as string):
		print _x
		_x = x
		self()

class B(A):
	_y = 1

	def constructor():
		print _y

	def constructor(y as int):
		print _y
		_y = y
		self()

A("post")
B(42)

