"""
public abstract class Foo(object):

	public abstract def Pub() as void:
		pass

	protected abstract def Pro() as void:
		pass

	private abstract def Pri() as void:
		pass

	internal abstract def Int() as void:
		pass

	protected def constructor():
		super()

public class Bar(Foo):

	public def constructor():
		super()

	public override def Pub() as void:
		raise System.NotImplementedException()

	protected override def Pro() as void:
		raise System.NotImplementedException()

	private override def Pri() as void:
		raise System.NotImplementedException()

	internal override def Int() as void:
		raise System.NotImplementedException()
"""
class Foo:
	public abstract def Pub():
		pass
	protected abstract def Pro():
		pass
	private abstract def Pri():
		pass
	internal abstract def Int():
		pass

class Bar(Foo):
	pass

