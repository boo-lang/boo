"""
01 02 03 01 02 03
1 1 | 2 2 | 3 3
"""

import System.Linq

ints as int* = (1, 2, 3, 1, 2, 3)

print join(ints.Select({i | i.ToString("00")}))
print join(join(g) for g in ints.ToLookup({i | return i}), " | ")
