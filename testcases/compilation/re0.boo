"""
255
255
0
"""
_, r, g, b = /color\((\d+),\w*(\d+),\w*(\d+)\)/.Match("color(255, 255, 0)").Groups
print(r.Value)
print(g.Value)
print(g.Value)
 
