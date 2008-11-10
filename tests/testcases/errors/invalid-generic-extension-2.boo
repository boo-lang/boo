"""
invalid-generic-extension-2.boo(11,4): BCE0019: 'Test' is not a member of 'int'.
"""

import System.Collections.Generic

[Extension]
def Test[of T](source as IEnumerable[of T]):
	print "You should not be here. -- Levelord"
	
42.Test()
