"""
01 02 03 01 02 03
1 1 | 2 2 | 3 3
"""

import System.Linq.Enumerable from "System.Core.dll"

ints as int* = (of int: 1,2,3,1,2,3)

print join(ints.Select({i as int | i.ToString("00")}))
print join(join(g) for g in ints.ToLookup({i as int | return i}), " | ")
