"""
2
4
"""
namespace Testing
import System

[Extension]
def IndexWhere[of T]([Required] coll as T*, [Required] filter as Func[of T, bool]) as int*:
	return IndexWhereImpl(coll, filter)

private def IndexWhereImpl[of T](coll as T*, filter as Func[of T, bool]) as int*:
	index = 0
	for value in coll:
		if filter(value):
			yield index
		++index

for index in ('a', 'b', '', 'd', null, 'f', 'g').IndexWhere(string.IsNullOrEmpty):
	print index;