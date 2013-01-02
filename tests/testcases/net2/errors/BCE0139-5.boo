"""
BCE0139-5.boo(14,13): BCE0139: 'Container[of T, U]' requires '2' arguments.
BCE0139-5.boo(15,26): BCE0139: 'System.Collections.Generic.List[of T]' requires '1' arguments.
BCE0139-5.boo(15,13): BCE0139: 'Container[of T, U]' requires '2' arguments.
"""
import System.Collections.Generic

class Container[of T, U]:
	public value as T

c = Container[of object, int]()


print c isa Container
print c isa Container of List

