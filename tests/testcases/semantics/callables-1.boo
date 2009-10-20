"""
[System.Runtime.CompilerServices.CompilerGeneratedAttribute]
public final class IntFunction(System.MulticastDelegate, callable):

	// runtime
	public def constructor(instance as object, method as System.IntPtr):
		pass

	public virtual def Call(args as (object)) as object:
		return self.Invoke(args[0])

	// runtime
	public virtual def Invoke(i as int) as int:
		pass

	// runtime
	public virtual def BeginInvoke(i as int, callback as System.AsyncCallback, asyncState as object) as System.IAsyncResult:
		pass

	// runtime
	public virtual def EndInvoke(asyncResult as System.IAsyncResult) as int:
		pass

[System.Runtime.CompilerServices.CompilerGlobalScopeAttribute]
public final transient class Callables_1Module(object):

	public static def square(i as int) as int:
		return (i * i)

	private static def Main(argv as (string)) as void:
		fn = IntFunction(null, __addressof__(Callables_1Module.square))
		fn.Invoke(2)

	private def constructor():
		super()
"""
callable IntFunction(i as int) as int

def square(i as int):
	return i*i

fn as IntFunction = square
fn(2)

