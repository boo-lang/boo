"""
2 20
1 a
"""
import System
import System.Collections.Generic
import System.Linq.Enumerable

class Table[of T]:
	def Index(pairs as KeyValuePair[of int, T]*):
		index = pairs.ToDictionary({kv | kv.Key}, {kv | kv.Value})
		print index.Count, index[1]

# ToDictionary has overloads taking a comparer and an element selector in the
# third position, so each closure is offered to several candidates before one is
# chosen. A closure bound once per candidate used to read its own parameter as a
# captured variable.
values = (1, 2)
d = values.ToDictionary({a | a}, {b | b * 10})
print d.Count, d[2]

pairs = List[of KeyValuePair[of int, string]]()
pairs.Add(KeyValuePair[of int, string](1, "a"))
pairs.Add(KeyValuePair[of int, string](2, "b"))
Table[of string]().Index(pairs.Take(1))
