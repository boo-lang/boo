"""
3
"""

import System.Collections.Generic

items = List of int((1,2,3))
count = items.ConvertAll[of string]({i as int  | i.ToString("00")}).Count
print count
