"""
Thingy
"""

import System.Collections.Generic

class Thingy:
	_thingies = List of Thingy()
	
	public AsList as List of Thingy:
		get: return _thingies

	public AsIList as IList of Thingy:
		get: return _thingies

	public AsICollection as ICollection of Thingy:
		get: return _thingies

	public AsNonGeneric as System.Collections.ICollection:
		get: return _thingies

t = Thingy()
print t
