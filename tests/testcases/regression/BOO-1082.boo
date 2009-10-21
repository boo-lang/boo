import System.Reflection

abstract class A:
	protected abstract def A():
		pass
		
	internal abstract def B():
		pass
		
	public abstract def C():
		pass

abstract class B(A):
	pass

class C(B):
	protected override def A():
		pass


assert typeof(B).GetMethod("A", BindingFlags.NonPublic | BindingFlags.Instance).IsFamily
assert typeof(B).GetMethod("B", BindingFlags.NonPublic | BindingFlags.Instance).IsAssembly
assert typeof(B).GetMethod("C").IsPublic

