"""
BCW0011-19.boo(14,16): BCW0011: WARNING: Type 'C' does not provide an implementation for 'A.P', a stub has been created.
BCW0011-19.boo(17,16): BCW0011: WARNING: Type 'D' does not provide an implementation for 'BooCompiler.Tests.SupportingClasses.A1.P1', a stub has been created.
"""
import BooCompiler.Tests.SupportingClasses from BooCompiler.Tests

public abstract class A:
	protected abstract P as string:
		get

public abstract class B(A):
	pass

public class C(B):
	pass

public class D(A2):
	pass