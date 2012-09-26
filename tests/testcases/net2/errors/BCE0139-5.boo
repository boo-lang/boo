"""
BCE0139-5.boo(14,13): BCE0177: Using the generic type 'Container[of T, U]' requires 2 type arguments.
BCE0139-5.boo(15,26): BCE0177: Using the generic type 'System.Collections.Generic.List[of T]' requires 1 type arguments.
BCE0139-5.boo(15,13): BCE0139: 'Container[of T, U]' requires '2' arguments.
"""
import System.Collections.Generic

class Container[of T, U]:
	public value as T

c = Container[of object, int]()


print c isa Container
print c isa Container of List

