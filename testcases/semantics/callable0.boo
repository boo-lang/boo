"""
public final transient class Callable0Module(System.Object):

	public static def ToUpper(item as System.String) as System.String:
		return item.ToUpper()

	private static def __Main__():
		Boo.Lang.Builtins.print(Boo.Lang.Builtins.join(Boo.Lang.Builtins.map(Callable0Module.__Callable1__.Instance, ['foo', 'bar'])))
		Boo.Lang.Builtins.print(Boo.Lang.Builtins.join(Boo.Lang.Builtins.map(Callable0Module.__Callable1__.Instance, ('bar', 'foo'))))

	private def constructor():
		super()

	privatescope final class __Callable1__(System.Object, Boo.Lang.ICallable):

		privatescope Instance as Callable0Module.__Callable1__

		public virtual def Call(args as (System.Object)) as System.Object:
			if (1 != args.Length):
				raise Boo.Lang.ArgumentCountException('Callable0Module.ToUpper', 1, args.Length)
			return Callable0Module.ToUpper(args[0])

		private def constructor():
			super()
			
		public static def constructor():
			Callable0Module.__Callable__1.Instance = Callable0Module.__Callable1__()

"""
def ToUpper(item as string):
	return item.ToUpper()

print(join(map(ToUpper, ["foo", "bar"])))
print(join(map(ToUpper, ("bar", "foo"))))
