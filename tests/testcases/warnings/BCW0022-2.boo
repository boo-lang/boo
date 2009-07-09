"""
BCW0022-2.boo(31,23): BCW0022: WARNING: Boolean expression will always have the same value.
BCW0022-2.boo(32,38): BCW0022: WARNING: Boolean expression will always have the same value.
BCW0022-2.boo(44,4): BCW0022: WARNING: Boolean expression will always have the same value.
BCW0022-2.boo(46,4): BCW0022: WARNING: Boolean expression will always have the same value.
BCW0022-2.boo(48,4): BCW0022: WARNING: Boolean expression will always have the same value.
BCW0022-2.boo(50,12): BCW0022: WARNING: Boolean expression will always have the same value.
BCW0022-2.boo(52,12): BCW0022: WARNING: Boolean expression will always have the same value.
BCW0022-2.boo(54,24): BCW0022: WARNING: Boolean expression will always have the same value.
BCW0022-2.boo(58,13): BCW0022: WARNING: Boolean expression will always have the same value.
BCW0022-2.boo(60,4): BCW0022: WARNING: Boolean expression will always have the same value.
BCW0022-2.boo(62,4): BCW0022: WARNING: Boolean expression will always have the same value.
BCW0022-2.boo(64,4): BCW0022: WARNING: Boolean expression will always have the same value.
BCW0022-2.boo(66,14): BCW0022: WARNING: Boolean expression will always have the same value.
BCW0022-2.boo(70,15): BCW0022: WARNING: Boolean expression will always have the same value.
BCW0022-2.boo(72,15): BCW0022: WARNING: Boolean expression will always have the same value.
BCW0022-2.boo(93,12): BCW0022: WARNING: Boolean expression will always have the same value.
"""
class Constant:
	public static x = 0
	public static final y = 1
	public static final z as int
	public static final s = "FOO"
	event FooEvent as callable()

	static def constructor():
		z = 42

	def Foo():
		FooEvent() if FooEvent #OK (event)
		FooEvent() if Foo
		FooEvent() if System.Console.WriteLine


def NotAProperty():
	return false

def GenericNotAProperty[of T]():
	return false

b = false
s = "bar"

if true:
	pass
if 1:
	pass
if not 1:
	pass
if 1 and 2 == 1:
	pass
if 1 and 2 or not 3:
	pass
if not b == false or 1 == 31:
	pass
#if "foo": #FIXME: handle BOO-1035
#	pass
if Constant.y:
	pass
if not Constant.y:
	pass
if not NotAProperty: #FAIL (!!implicit callable!!)
	pass
if GenericNotAProperty: #same
	pass
if not "foo" is "bar":
	pass
if "foo" isa string:
	pass
if Constant.s is not null:
	pass
if Constant.s is "FOO":
	pass


if 1 and not b: #OK
	pass
if Constant.x: #OK
	pass
if Constant.z: #OK
	pass
if b: #OK
	pass
if NotAProperty(): #OK (invocation)
	pass
if GenericNotAProperty[of int](): #OK
	pass
if s is "foo": #OK
	pass
if s is not null: #OK
	pass

assert 4*4 == 4+4

