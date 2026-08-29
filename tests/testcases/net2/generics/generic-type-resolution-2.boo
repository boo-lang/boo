"""
False
True
"""

import System
import System.Collections
import System.Collections.Generic

c1 = Comparer.Default
c2 = Comparer[of string].Default

print c1.GetType().IsGenericType
print c2.GetType().IsGenericType

