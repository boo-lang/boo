"""
01 02 03
"""

import System
import System.Collections.Generic

callable Transformation[of TIn, TOut](arg as TIn) as TOut

[Extension]
def Map[of TIn, TOut](source as IEnumerable[of TIn], transform as Transformation[of TIn, TOut]) as IEnumerable[of TOut]:
	result = List[of TOut]()
	for item in source:
		result.Add(transform(item))
	return result
	
numbers = List of int((1,2,3))
mapped = numbers.Map({ i as int | i.ToString("00") })
assert mapped isa IEnumerable of string
print join(mapped)