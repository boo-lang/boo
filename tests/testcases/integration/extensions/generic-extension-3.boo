"""
01 => A
02 => B
03 => C
"""

import System
import System.Collections.Generic

callable Transform[of TIn, TOut](arg as TIn) as TOut

[Extension]
def ToHash[of T, U](keys as IList[of T], values as IList[of U]):
	result = Dictionary[of T, U](keys.Count)
	for i in range(Math.Min(keys.Count, values.Count)):
		result.Add(keys[i], values[i])
	return result
	
numbers = List of int((1,2,3))
strings = List of string (("a", "b", "c"))

hash = numbers.ToHash(strings)
for kvp in hash:
	print "${kvp.Key.ToString('00')} => ${kvp.Value.ToUpper()}"
