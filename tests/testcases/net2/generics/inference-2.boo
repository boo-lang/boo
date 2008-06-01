"""
Int32
"""

import System.Collections.Generic

def Method[of T](arg as IEnumerable of T):
	return typeof(T).Name

arg as IEnumerable of int	
print Method(arg)