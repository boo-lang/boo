"""
public final transient class Callable0Module(System.Object):

	public static def ToUpper(item as System.String) as System.String:
		return item.ToUpper()

	private static def __Main__():
		print(map(__CallableToUpper__(), ['foo', 'bar']))

	private def constructor():
		super()

	internal class __CallableToUpper__(System.Object, Boo.Lang.ICallable):

		def Call(args as (object)):
			raise ArgumentCountException('Callable0Module.ToUpper', 1, args.Length) unless 1 == args.Length
			return Callable0Module.ToUpper(args[0])
"""
def ToUpper(item as string):
	return item.ToUpper()

print(map(ToUpper, ["foo", "bar"]))
