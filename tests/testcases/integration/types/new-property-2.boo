"""
42 1
"""
import System.Collections.Generic

class Counted(Dictionary[of int, string]):
	public Count as int:
		new get:
			return 42

counted = Counted()
counted[1] = "a"
print counted.Count, (counted cast Dictionary[of int, string]).Count
