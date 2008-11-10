"""
42
"""

import System.Collections.Generic

[Extension]
def CountItems[of T](target as IEnumerable[of T]):
	return List[of T](target).Count
	
items = i for i in range(42)
print items.CountItems()
