"""
[Boo.Lang.BooModuleAttribute]
public final transient class Array_functionModule(System.Object):

	private static def __Main__(argv as (System.String)) as System.Void:
		for a as System.Int32 in cast((System.Int32), Boo.Lang.Builtins.array(System.Int32, Boo.Lang.Builtins.range(10))):
			Boo.Lang.Builtins.print(a)
		for a as System.Int32 in cast((System.Int32), Boo.Lang.Builtins.array(System.Int32, Boo.Lang.Builtins.range(10))):
			Boo.Lang.Builtins.print(a)
		for a as System.String in cast((System.String), Boo.Lang.Builtins.array(System.String, ['foo', 'bar'])):
			Boo.Lang.Builtins.print(a)

	private def constructor():
		super()
"""
for a in array(int, range(10)):
	print(a)
	
for a in array(range(10)):
	print(a)

for a in array(string, ["foo", "bar"]):
	print(a)
