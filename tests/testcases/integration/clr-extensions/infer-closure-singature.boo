"""
0 -1
"""
import System.Linq.Enumerable

print join(("1", "2").Select({s|s.IndexOf("1")}).ToArray())