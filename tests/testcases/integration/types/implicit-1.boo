"""
{X=0, Y=0}
{X=0, Y=0}
"""

import System.Drawing from System.Drawing

class C:
	pf as PointF = Point(1,1)

def conv(p as PointF):
	print p

p = Point(0, 0)
conv(p)

pf as PointF = p
print pf

