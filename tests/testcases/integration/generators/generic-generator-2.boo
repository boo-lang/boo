#reported in https://github.com/boo-lang/boo/issues/123
"""
A
B
C
D
E
"""
import System.Collections.Generic
def ggm[of T](l as IEnumerable[of T]) as IEnumerator[of T]:
	for e in l:
		yield e

for line in ggm(("A", "B", "C", "D", "E")):
	print line