"""
BCE0007-1.boo(13,16): BCE0007: The name 'y' does not represent a settable public property or field of the type 'Foo'.
BCE0007-1.boo(13,22): BCE0007: The name 'z' does not represent a settable public property or field of the type 'Foo'.
"""


class Foo:
	public static x = 0
	public static final y = 0
	public final z = 0
	public ok = 1

_ = Foo(x: 42, y:42, z: 42, ok: 42)

