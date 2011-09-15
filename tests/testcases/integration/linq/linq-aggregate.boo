"""
6
"""
import System.Linq
a = (1, 2, 3)
print a.Aggregate({i, j | i + j})
