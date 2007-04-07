"""
System.Collections.Comparer
System.Collections.Generic.IComparableOfTComparer`1[System.String]
"""

import System
import System.Collections
import System.Collections.Generic

c1 = Comparer.Default
c2 = Comparer[of string].Default

print c1.GetType()
print c2.GetType()

