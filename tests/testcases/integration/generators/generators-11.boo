import System
import System.Collections

generator = i*2 for i in range(1, 10)

e1 = generator.GetEnumerator()

assert e1 isa ICloneable, "enumerator must be cloneable!"

assert e1.MoveNext()
assert 2 == e1.Current

e2 as IEnumerator = cast(ICloneable, e1).Clone()
assert not e1 is e2
assert 2 == e2.Current

assert e1.MoveNext()
assert 4 == e1.Current
assert 2 == e2.Current, "Clone must not be affected by original copy!"

