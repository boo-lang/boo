"""
ffffff00
16777215
00ffffff
"""

c as uint = uint.MaxValue-byte.MaxValue
print c.ToString("x8") #OK ffffff00
print c >> 8           #! -1 should be 16777215 (00ffffff)
c = c >> 8
print c.ToString("x8")  #! overflow should be 00ffffff

