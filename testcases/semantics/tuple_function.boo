"""
public final transient class Tuple_functionModule(System.Object):

	private static def __Main__(argv as (System.String)) as System.Void:
		for a as System.Int32 in Boo.Lang.Builtins.tuple(System.Int32, Boo.Lang.Builtins.range(10)):
			Boo.Lang.Builtins.print(a)
		for a as System.Object in Boo.Lang.Builtins.tuple(Boo.Lang.Builtins.range(10)):
			Boo.Lang.Builtins.print(a)
		for a as System.String in Boo.Lang.Builtins.tuple(System.String, ['foo', 'bar']):
			Boo.Lang.Builtins.print(a)

	private def constructor():
		super()
"""
for a in tuple(int, range(10)):
	print(a)
	
for a in tuple(range(10)):
	print(a)

for a in tuple(string, ["foo", "bar"]):
	print(a)
