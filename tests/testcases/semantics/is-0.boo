"""
public class Test(System.Object):

	public static def IsNotNull(o as System.Object) as System.Boolean:
		return (o is not null)

	public static def IsNull(o as System.Object) as System.Boolean:
		return (o is null)

	public def constructor():
		super()
"""
class Test:

	static def IsNotNull(o):
		return o != null
		
	static def IsNull(o):
		return o == null
