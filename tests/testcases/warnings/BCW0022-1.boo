"""
BCW0022-1.boo(55,26): BCW0022: WARNING: Boolean expression will always have the same value.
BCW0022-1.boo(56,18): BCW0022: WARNING: Boolean expression will always have the same value.
BCW0022-1.boo(57,18): BCW0022: WARNING: Boolean expression will always have the same value.
BCW0022-1.boo(64,26): BCW0022: WARNING: Boolean expression will always have the same value.
BCW0022-1.boo(65,26): BCW0022: WARNING: Boolean expression will always have the same value.
BCW0022-1.boo(66,18): BCW0022: WARNING: Boolean expression will always have the same value.
BCW0022-1.boo(67,18): BCW0022: WARNING: Boolean expression will always have the same value.
BCW0022-1.boo(78,54): BCW0022: WARNING: Boolean expression will always have the same value.
BCW0022-1.boo(78,26): BCW0022: WARNING: Boolean expression will always have the same value.
BCW0022-1.boo(79,19): BCW0022: WARNING: Boolean expression will always have the same value.
BCW0022-1.boo(81,15): BCW0022: WARNING: Boolean expression will always have the same value.
BCW0022-1.boo(94,9): BCW0022: WARNING: Boolean expression will always have the same value.
BCW0022-1.boo(95,12): BCW0022: WARNING: Boolean expression will always have the same value.
BCW0022-1.boo(96,12): BCW0022: WARNING: Boolean expression will always have the same value.
BCW0022-1.boo(97,18): BCW0022: WARNING: Boolean expression will always have the same value.
BCW0022-1.boo(98,18): BCW0022: WARNING: Boolean expression will always have the same value.
BCW0022-1.boo(99,15): BCW0022: WARNING: Boolean expression will always have the same value.
BCW0022-1.boo(100,18): BCW0022: WARNING: Boolean expression will always have the same value.
BCW0022-1.boo(102,16): BCW0022: WARNING: Boolean expression will always have the same value.
BCW0022-1.boo(104,33): BCW0022: WARNING: Boolean expression will always have the same value.
BCW0022-1.boo(105,44): BCW0022: WARNING: Boolean expression will always have the same value.
"""
enum Enum:
	Foo
	Bar
class Constant:
	public static x = 0
	public static final y = 1
	public static final z = 2
class Constant2:
	public static final y = 1
	public static final z = 2

class NonConstant:
	public static final y = 1
	public static final z = 2

	static def constructor():
		y = 42

class Test:
	public x = 0

	public final y = 1
	public final z = 2

	public static final sy = 1
	public static final sz = 2
	public static final SF as int #NOT A LITERAL (set in static constructor)
	public static Color as System.ConsoleColor
	test as Test

	static def constructor():
		print Constant.y <= Constant2.y
		print sy == sz
		print sy != Constant.y
		print SF == sy #OK (SF not a literal final)
		print Constant.x > Constant.y #OK (x not final)
		print NonConstant.y != Constant.y #OK
		SF = 1

	def constructor():
		print Constant.y != Constant.z
		print Constant.y > Constant2.y
		print sy == Constant2.y
		print sy < sz
		print NonConstant.y >= Constant.y #OK
		print SF >= Constant2.y #OK (SF not a literal final)
		print x == y #OK
		print test.y == z #OK (not self)
		print test.y == test.z #OK (not self)
		print y >= z #OK (not static)
		print y != sy #OK (y not static, sy has static constructor)
		print y > Constant.y #OK (y not static)

	def Foo():
		print Constant.y < Constant2.y if Constant.y == Constant.z
		if not sy == Constant2.y:
			pass
		if sy < sz:
			pass
		print Constant.y != NonConstant.y #OK
		print NonConstant.y == NonConstant.z #OK
		print x != y #OK
		print test.y == z #OK
		print test.y == test.z #OK
		print self.y < self.z #OK
		print y >= z #OK
		print y != sy #OK


t = Test()
print 1 == 42
print true != true
print 0x22 > 50l
print Constant.y > Constant2.y
print Constant.z <= Constant2.y
print Test.sy != Test.sz
print Constant.y > Test.sy

print Enum.Foo == Enum.Bar
print Constant.y > Test.SF #OK (SF not literal final)
print System.ConsoleColor.Black != System.ConsoleColor.White
print cast(int, System.ConsoleColor.Black) != cast(int, System.ConsoleColor.White)
color = System.ConsoleColor.Black
print color != System.ConsoleColor.White #OK
if Test.Color == System.ConsoleColor.Black:
	pass
print t.y == t.z #OK (not self)
print t.x == Constant2.y #OK

