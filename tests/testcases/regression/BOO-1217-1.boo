class Foo[of T]:
	public x as int

	def constructor(x as int):
		.x = x

	static def op_Addition(left as Foo[of T], right as int) as Foo[of T]:
		left.x += right
		return left

	def SameSame() as Foo[of T]:
		return null


fv = Foo[of System.Attribute](1)
fv += 1
assert fv.x == 2

assert fv.SameSame() == null

