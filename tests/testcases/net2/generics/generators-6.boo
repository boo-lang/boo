"""
Thingy 2
Thingy 4
Thingy 6
"""

class Thingy:
	_value as int

	public def constructor(val as int):
		_value = val

	public override def ToString():
		return "Thingy ${_value}"

e = Thingy(i * 2) for i in (1,2,3,4) if i <= 3
for t in e: print t
