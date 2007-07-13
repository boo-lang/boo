"""
public class Test(object):

	public static def IsNotNull(o as object) as bool:
		return (o is not null)

	public static def IsNull(o as object) as bool:
		return (o is null)

	public def constructor():
		super()
"""
class Test:

	static def IsNotNull(o):
		return o != null
		
	static def IsNull(o):
		return o == null
