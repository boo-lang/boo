"""
01
02
03
04
"""

import System

ints = (1,2,3,4)
strings = Array.ConvertAll[of int, string](ints, {i as int | i.ToString("00")})
for s in strings: print s
