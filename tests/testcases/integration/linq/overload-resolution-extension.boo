"""
1
"""
import System.Collections.Generic
import System.Linq

class TestList[of T](List[of T]):
	public First as T:
		get:
			self[0]
			
list = TestList of int()
list.Add(1)
print list.First()