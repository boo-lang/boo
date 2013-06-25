"""
BCW0011-18.boo(13,16): BCW0011: WARNING: Type 'C' does not provide an implementation for 'B.L', a stub has been created.
"""
public abstract class A:
	public virtual L as string:
		get

public abstract class B(A):
	
	public abstract override L as string:
		get

public class C(B):
	pass

