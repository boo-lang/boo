"""
255
255
0
"""
import System.Text.RegularExpressions

_, r as Group, g as Group, b as Group = /color\((\d+),\s*(\d+),\s*(\d+)\)/.Match("color(255, 255, 0)").Groups
print(r.Value)
print(g.Value)
print(b.Value)
